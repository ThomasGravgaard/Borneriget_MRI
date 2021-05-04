using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Borneriget.MRI
{
    public class Story2dView : MonoBehaviour, IPointerDownHandler, IStoryView, IBeginDragHandler, IDragHandler, IEndDragHandler, IVideoControl
    {
        [SerializeField]
        private Camera MenuCam;
        [SerializeField]
        private RawImage Background;
        [SerializeField]
        private Texture[] BackgroundImages;
        [SerializeField]
        private GameObject Bear;
        [SerializeField]
        private GameObject VideoImage;
        [SerializeField]
        private Image VideoProgress;
        [SerializeField]
        private GameObject ExitButton;
        [SerializeField]
        private GameObject NextButton;
        [SerializeField]
        private GameObject Spinner;
        [SerializeField]
        private float SpinnerRotateSpeed = -180;
        [SerializeField]
        private Animator RoomIconAnimation;
        [SerializeField]
        private RawImage RoomIcon;
        [SerializeField]
        private Text RoomText;
        [SerializeField]
        private Texture[] Icon;

        [SerializeField]
        private MenuView Menu;
        [SerializeField]
        private float dragYrange;

        [SerializeField]
        private Animator Animator;

        public event Action<int> SelectRoom;
        public event Action Exit;
        public event Action StartSeek;
        public event Action<float> SetSeekPosition;
        public event Action EndSeek;
        private bool avatarClicked;

        private bool videoControlsEnabled => VideoImage.activeInHierarchy && VideoProgress.fillAmount > 0;

        private void Awake()
        {
            Application.unloading += Application_unloading;
            VideoProgress.fillAmount = 0;
            Background.gameObject.SetActive(false);
            Bear.SetActive(false);
            Menu.MenuSelected += Button_Click;
            Menu.Hide();
            VideoImage.SetActive(false);
            ExitButton.SetActive(false);
            NextButton.SetActive(false);
            Spinner.SetActive(false);
        }

        private void OnDestroy()
        {
            Menu.MenuSelected -= Button_Click;
            Application.unloading -= Application_unloading;
        }

        private void Application_unloading()
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        public void Initialize(bool isDanish, string doneNotification)
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Menu.Initialize(isDanish);
            StartCoroutine(InitializeCo(doneNotification));
        }

        private IEnumerator InitializeCo(string doneNotification)
        {
            yield return null;
            Bootstrap.Facade.SendNotification(doneNotification);
        }

        public void Show(int room, string doneNotification, bool avatarAwake)
        {
            avatarClicked = avatarAwake;
            VideoProgress.fillAmount = 0;
            ExitButton.SetActive(avatarAwake);
            NextButton.SetActive(avatarAwake);
            VideoImage.SetActive(false);
            Spinner.SetActive(false);
            MenuCam.enabled = true;
            Background.gameObject.SetActive(true);
            Bear.SetActive(true);
            if (string.IsNullOrEmpty(doneNotification))
            {
                // We have no notification, so we will show the menu and wait for a click.
                Background.texture = BackgroundImages.SafeGet(0);
                Menu.Show();
            }
            else
            {
                Background.texture = BackgroundImages.SafeGet(room);
                StartCoroutine(ShowCo(doneNotification));
                Menu.Hide();
            }
            var icon = Icon.SafeGet(room);
            if (icon)
            {
                // We have an icon, so set its properties and show it
                RoomText.text = Menu.GetText(room);
                RoomIcon.texture = icon;
                RoomIconAnimation.SetTrigger("Fade");
            }
        }

        private IEnumerator ShowCo(string doneNotification)
        {
            yield return new WaitForSeconds(1f);
            Bootstrap.Facade.SendNotification(doneNotification);
            yield return new WaitForSeconds(10f);
            if (!avatarClicked)
            {
                Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
            }
        }

        public void Hide()
        {
            Background.gameObject.SetActive(false);
            Bear.SetActive(false);
            Menu.Hide();
            VideoImage.SetActive(false);
            ExitButton.SetActive(false);
            NextButton.SetActive(false);
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }

        public void ShowSpinner()
        {
            Spinner.SetActive(true);
        }

        public void ShowButtons()
        {
            Menu.Show();
        }

        private void Button_Click(int index)
        {
            Menu.Hide();
            SelectRoom?.Invoke(index);
        }

        private IEnumerator ShowButtonsAfterAwake()
        {
            yield return new WaitForSeconds(5);
            ExitButton.SetActive(true);
            NextButton.SetActive(true);
            avatarClicked = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (!avatarClicked)
            {
                // The bear is not awake, so the only controls active are the bear
                if (target == Bear)
                {
                    Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
                    StartCoroutine(ShowButtonsAfterAwake());
                }
            }
            else
            {
                if (target == ExitButton)
                {
                    Exit?.Invoke();
                }
                else if (target == NextButton)
                {
                    Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
                    if (VideoImage.activeInHierarchy)
                    {
                        Bootstrap.Facade.SendNotification(VideoMediator.Notifications.StopVideo);
                    }
                    else
                    {
                        Bootstrap.Facade.SendNotification(AvatarMediator.Notifications.StopSpeak);
                    }
                }
                else if (videoControlsEnabled)
                {
                    if (eventData.button == PointerEventData.InputButton.Left && eventData.position.y > dragYrange)
                    {
                        // We are playing a video. A click will pause it
                        Bootstrap.Facade.SendNotification(VideoMediator.Notifications.TogglePause);
                        Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
                    }
                    if (eventData.button == PointerEventData.InputButton.Right)
                    {
                        // We are playing a video. A right click will stop it
                        Bootstrap.Facade.SendNotification(VideoMediator.Notifications.StopVideo);
                    }
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    // We are in a speak. Right click to skip it.
                    Bootstrap.Facade.SendNotification(AvatarMediator.Notifications.StopSpeak);
                }
            }
        }

        public void ShowVideo()
        {
            Bear.SetActive(false);
            Background.gameObject.SetActive(false);
            VideoImage.SetActive(true);
        }

        public void SetVideoProgress(VideoProgress progress)
        {
            VideoProgress.fillAmount = progress.Progress;
            Spinner.SetActive(false);
        }

        public void ShowPause()
        {
            Animator.SetTrigger("Pause");
        }

        public void ShowResume()
        {
            Animator.SetTrigger("Resume");
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (videoControlsEnabled && eventData.position.y <= dragYrange)
            {
                var position = eventData.position.x / Screen.width;
                SetSeekPosition?.Invoke(position);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (videoControlsEnabled && eventData.position.y <= dragYrange)
            {
                Animator.SetBool("ShowTimeline", true);
                StartSeek?.Invoke();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (videoControlsEnabled)
            {
                Animator.SetBool("ShowTimeline", false);
                EndSeek?.Invoke();
            }
        }

        public void Update()
        {
            if (Spinner.activeInHierarchy)
            {
                Spinner.transform.Rotate(Vector3.forward, SpinnerRotateSpeed * Time.deltaTime);
            }
        }
    }
}
