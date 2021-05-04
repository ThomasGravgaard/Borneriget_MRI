using Google.XR.Cardboard;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Management;

namespace Borneriget.MRI
{
    public class Story3dView : MonoBehaviour, IStoryView
    {
        [SerializeField]
        private GameObject Bear;
        [SerializeField]
        private Renderer ScreenImage;
        [SerializeField]
        private Texture[] ScreenImages;
        [SerializeField]
        private Texture[] Icon;
        [SerializeField]
        private GameObject Environment;
        [SerializeField]
        private Camera MenuCam;
        [SerializeField]
        private Camera Cam;
        [SerializeField]
        private GameObject CamRoot;
        [SerializeField]
        private Image GazeProgress;
        [SerializeField]
        private float ProgressSpeed = 1.5f;
        [SerializeField]
        private float MouseRotateSpeed = 45f;
        [SerializeField]
        private MenuView Menu;
        [SerializeField]
        private GameObject[] MenuColliders;
        [SerializeField]
        private GameObject Spinner;
        [SerializeField]
        private float SpinnerRotateSpeed = -180;

        private Vector3 lastMousePos;
        private bool vrPlaying = false;
        private float progress;
        private bool avatarAwake;

        public event Action Exit;
        public event Action<int> SelectRoom;

        private void Awake()
        {
            Environment.SetActive(false);
            CamRoot.gameObject.SetActive(false);
            GazeProgress.gameObject.SetActive(false);
            Spinner.SetActive(false);
            Menu.Hide();
            GazeProgress.fillAmount = 0;
        }

        public void Initialize(bool isDanish, string doneNotification)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Menu.Initialize(isDanish);
            StartCoroutine(InitializeXRCo(doneNotification));
        }

        private IEnumerator InitializeXRCo(string doneNotification)
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                Debug.Log("Initializing XR Failed.");
            }
            else
            {
                if (!Api.HasDeviceParams())
                {
                    Api.ScanDeviceParams();
                }
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                vrPlaying = true;
            }
            Bootstrap.Facade.SendNotification(doneNotification);
        }

        public void Show(int room, string doneNotification, bool avatarAwake)
        {
            this.avatarAwake = avatarAwake;
#if UNITY_EDITOR
            lastMousePos = Input.mousePosition;
#endif
            ScreenImage.material.mainTexture = ScreenImages.SafeGet(room);
            ScreenImage.material.SetTexture("_EmissionMap", ScreenImages.SafeGet(room));
            MenuCam.enabled = false;
            CamRoot.gameObject.SetActive(true);
            Environment.SetActive(true);
            GazeProgress.gameObject.SetActive(true);
            Spinner.SetActive(false);
            StartCoroutine(ShowCo(doneNotification));
            if (string.IsNullOrEmpty(doneNotification))
            {
                // We have no notification, so we will show the menu and wait for a click.
                ScreenImage.enabled = false;
                Menu.Show();
            }
            else
            {
                ScreenImage.enabled = true;
                Menu.Hide();
            }
        }

        private IEnumerator ShowCo(string doneNotification)
        {
            Bear.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            Bootstrap.Facade.SendNotification(doneNotification);
            yield return new WaitForSeconds(10f);
            if (progress < 1)
            {
                Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
            }
        }

        public void ShowSpinner()
        {
            Spinner.SetActive(true);
        }

        public void SetVideoProgress(VideoProgress progress)
        {
            // Currently we are not showing video progress in 3d
            if (Spinner.activeInHierarchy)
            {
                Spinner.SetActive(false);
            }
        }

        public void ShowVideo()
        {
            Environment.SetActive(false);
            Bear.SetActive(false);
        }

        public void Hide()
        {
            MenuCam.enabled = true;
            CamRoot.gameObject.SetActive(false);
            Environment.SetActive(false);
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        private IEnumerator StopXR()
        {
            yield return new WaitForSeconds(1f);
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        public void Update()
        {
            if (vrPlaying)
            {
                if (Api.IsCloseButtonPressed)
                {
                    StartCoroutine(StopXR());
                    Exit?.Invoke();
                }

                if (Api.IsGearButtonPressed)
                {
                    Api.ScanDeviceParams();
                }

                Api.UpdateScreenParams();

                var doRaycast = (Bear.activeInHierarchy && !avatarAwake) || Menu.gameObject.activeInHierarchy;

                if (doRaycast)
                {
                    // See if we are looking at the bear
                    var progressDelta = 0f;
                    RaycastHit hit;
                    if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit))
                    {
                        progressDelta = (1 / ProgressSpeed) * Time.deltaTime;
                    }
                    if (progressDelta > 0)
                    {
                        if (progress < 1)
                        {
                            progress = Mathf.Clamp01(progress + progressDelta);
                            if (GazeProgress)
                            {
                                GazeProgress.fillAmount = progress;
                            }
                            if (progress == 1)
                            {
                                GazeProgress.fillAmount = 0;
                                if (hit.collider.gameObject == Bear)
                                {
                                    Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
                                    avatarAwake = true;
                                }
                                else
                                {
                                    var menuItem = Array.IndexOf(MenuColliders, hit.collider.gameObject);
                                    if (menuItem > 0)
                                    {
                                        SelectRoom?.Invoke(menuItem);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        progress = 0;
                    }
                }

                if (Spinner.activeInHierarchy)
                {
                    Spinner.transform.Rotate(Vector3.forward, SpinnerRotateSpeed * Time.deltaTime);
                }
#if UNITY_EDITOR
                var mouseDelta = lastMousePos - Input.mousePosition;
                lastMousePos = Input.mousePosition;
                var rotate = new Vector3(mouseDelta.y, -mouseDelta.x, 0) * Time.deltaTime * MouseRotateSpeed;
                CamRoot.transform.Rotate(rotate);

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StopXR();
                    Exit?.Invoke();
                }
#endif
            }
        }
    }
}
