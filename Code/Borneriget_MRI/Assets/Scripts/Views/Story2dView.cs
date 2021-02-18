using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Borneriget.MRI
{
    public class Story2dView : MonoBehaviour, IPointerClickHandler, IStoryView
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
        private GameObject Buttons;
        [SerializeField]
        private Button Room1;
        [SerializeField]
        private Button Room2;

        public event Action<int> SelectRoom;
        public event Action Exit;

        private void Awake()
        {
            VideoProgress.fillAmount = 0;
            Background.gameObject.SetActive(false);
            Bear.SetActive(false);
            Buttons.SetActive(false);
            VideoImage.SetActive(false);
            ExitButton.SetActive(false);
            Room1.onClick.AddListener(Room1_Click);
            Room2.onClick.AddListener(Room2_Click);
        }

        public void Initialize(string doneNotification)
        {
            StartCoroutine(InitializeCo(doneNotification));
        }

        private IEnumerator InitializeCo(string doneNotification)
        {
            yield return null;
            Bootstrap.Facade.SendNotification(doneNotification);
        }

        public void Show(int room, string doneNotification)
        {
            VideoProgress.fillAmount = 0;
            Background.texture = BackgroundImages.SafeGet(room);
            StartCoroutine(ShowCo(doneNotification));
        }

        private IEnumerator ShowCo(string doneNotification)
        {
            ExitButton.SetActive(true);
            VideoImage.SetActive(false);
            MenuCam.enabled = true;
            Background.gameObject.SetActive(true);
            Bear.SetActive(true);
            yield return new WaitForSeconds(1f);
            Bootstrap.Facade.SendNotification(doneNotification);
        }

        public void Hide()
        {
            Background.gameObject.SetActive(false);
            Bear.SetActive(false);
            Buttons.SetActive(false);
            VideoImage.SetActive(false);
            ExitButton.SetActive(false);
        }

        public void ShowButtons()
        {
            Buttons.SetActive(true);
        }

        private void Room1_Click()
        {
            SelectRoom?.Invoke(1);
            gameObject.SetActive(false);
        }

        private void Room2_Click()
        {
            SelectRoom?.Invoke(2);
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (VideoImage.activeInHierarchy)
            {
                // We are playing a video. A click will pause it
                Bootstrap.Facade.SendNotification(VideoMediator.Notifications.TogglePause);
            }
            else
            {
                var target = eventData.pointerCurrentRaycast.gameObject;
                if (target == Bear)
                {
                    Bootstrap.Facade.SendNotification(StoryMediator.Notifications.AvatarClicked);
                }
                if (target == ExitButton)
                {
                    Exit?.Invoke();
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
        }
    }
}
