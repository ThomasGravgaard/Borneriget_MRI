using Google.XR.Cardboard;
using System;
using System.Collections;
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

        private Vector3 lastMousePos;
        private bool vrPlaying = false;
        private float progress;

        private void Awake()
        {
            Environment.SetActive(false);
            CamRoot.gameObject.SetActive(false);
            GazeProgress.gameObject.SetActive(false);
            GazeProgress.fillAmount = 0;
        }

        public void Initialize(string doneNotification)
        {
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

        public void Show(int room, string doneNotification)
        {
#if UNITY_EDITOR
            lastMousePos = Input.mousePosition;
#endif
            MenuCam.enabled = false;
            CamRoot.gameObject.SetActive(true);
            Environment.SetActive(true);
            GazeProgress.gameObject.SetActive(true);
            StartCoroutine(ShowCo(doneNotification));
        }

        private IEnumerator ShowCo(string doneNotification)
        {
            Bear.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            Bootstrap.Facade.SendNotification(doneNotification);
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
        }

        private void StopXR()
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }

        public void Update()
        {
            if (vrPlaying)
            {
                if (Api.IsCloseButtonPressed)
                {
                    StopXR();
                }

                if (Api.IsGearButtonPressed)
                {
                    Api.ScanDeviceParams();
                }

                Api.UpdateScreenParams();

                if (Bear.activeInHierarchy && progress < 1)
                {
                    // See if we are looking at the bear
                    var progressDelta = 0f;
                    if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out RaycastHit hit))
                    {
                        if (hit.transform.gameObject == Bear)
                        {
                            progressDelta = (1 / ProgressSpeed) * Time.deltaTime;
                        }
                    }
                    if (progressDelta > 0)
                    {
                        progress = Mathf.Clamp01(progress + progressDelta);
                    }
                    else
                    {
                        progress = 0;
                    }
                    if (GazeProgress)
                    {
                        GazeProgress.fillAmount = progress;
                    }
                    if (progress == 1)
                    {
                        GazeProgress.fillAmount = 0;
                        Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
                    }
                }
#if UNITY_EDITOR
                var mouseDelta = lastMousePos - Input.mousePosition;
                lastMousePos = Input.mousePosition;
                var rotate = new Vector3(mouseDelta.y, -mouseDelta.x, 0) * Time.deltaTime * MouseRotateSpeed;
                CamRoot.transform.Rotate(rotate);
#endif
            }
        }
    }
}
