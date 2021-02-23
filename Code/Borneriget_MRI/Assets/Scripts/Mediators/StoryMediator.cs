using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Borneriget.MRI
{
    public class StoryMediator : Mediator
    {
        public StoryMediator() : base(NAME) {}

        public new static string NAME = "StoryMediator";

        private IStoryView View => (IStoryView)ViewComponent;
        private PreferencesProxy Preferences;

        private bool AvatarAwake = false;
        private int Progress = 0;
        private bool ShowMenu = false;

        public static class Notifications
        {
            public const string ViewInitialized = "ViewInitialized";
            public const string ViewShown = "ViewShown";
            public const string AvatarClicked = "AvatarClicked";
            public const string FadeAfterVideo = "FadeAfterVideo";
            public const string FadeAfterMenuSelect = "FadeAfterMenuSelect";
            public const string ReturnToMenu = "ReturnToMenu";
        }

        public override string[] ListNotificationInterests()
        {
            return new[] {
                Notifications.ViewInitialized,
                Notifications.ViewShown,
                Notifications.AvatarClicked,
                Notifications.FadeAfterVideo,
                Notifications.FadeAfterMenuSelect,
                Notifications.ReturnToMenu,
                VideoMediator.Notifications.PlayVideo,
                VideoMediator.Notifications.VideoDone, 
                VideoMediator.Notifications.VideoProgressUpdate,
                AvatarMediator.Notifications.SpeakDone, 
                AvatarMediator.Notifications.AvatarAwake
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            Preferences = Facade.RetrieveProxy<PreferencesProxy>();

            Facade.RegisterMediator(new AvatarMediator(Preferences.Avatar));
            InitializeView();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.Exit -= View_Exit;
            View.SelectRoom -= View_SelectRoom;
        }

        private void InitializeView()
        {
            ViewComponent = Preferences.UseVr ? (IStoryView)Object.FindObjectOfType<Story3dView>(true) : (IStoryView)Object.FindObjectOfType<Story2dView>(true);
            View.Exit += View_Exit;
            View.SelectRoom += View_SelectRoom;
            View.Initialize(Notifications.ViewInitialized);
        }

        private void View_Exit()
        {
            SendNotification(FaderMediator.Notifications.StartFade, Notifications.ReturnToMenu);
        }

        private void View_SelectRoom(int index)
        {
            if (ShowMenu)
            {
                // We will start the speak and the video
                switch (index)
                {
                    case 0:
                        Progress = 1;
                        break;
                    case 1:
                        Progress = 2;
                        break;
                    case 2:
                        Progress = 4;
                        break;
                }
                SendNotification(FaderMediator.Notifications.StartFade, Notifications.FadeAfterMenuSelect);
            }
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.ViewInitialized:
                    View.Show(Progress, Notifications.ViewShown);
                    break;
                case Notifications.ViewShown:
                    SendNotification(VideoMediator.Notifications.PrepareVideo, Progress);
                    if (AvatarAwake)
                    {
                        SendNotification(AvatarMediator.Notifications.AvatarSpeak, Progress);
                    }
                    break;
                case Notifications.AvatarClicked:
                    if (!AvatarAwake)
                    {
                        SendNotification(AvatarMediator.Notifications.WakeAvatar);
                        AvatarAwake = true;
                    }
                    break;
                case AvatarMediator.Notifications.AvatarAwake:
                    SendNotification(AvatarMediator.Notifications.AvatarSpeak, Progress);
                    break;
                case AvatarMediator.Notifications.SpeakDone:
                    SendNotification(FaderMediator.Notifications.StartFade, new FaderMediator.FadeNotification {
                        Name = VideoMediator.Notifications.PlayVideo,
                        Body = Progress
                    });
                    break;
                case VideoMediator.Notifications.PlayVideo:
                    View.ShowVideo();
                    break;
                case VideoMediator.Notifications.VideoDone:
                    SendNotification(FaderMediator.Notifications.StartFade, Notifications.FadeAfterVideo);
                    break;
                case VideoMediator.Notifications.VideoProgressUpdate:
                    View.SetVideoProgress((VideoProgress)notification.Body);
                    break;
                case Notifications.FadeAfterVideo:
                    if (Progress++ == 5)
                    {
                        // We have shown all videos. We now have a selection menu.
                        ShowMenu = true;
                        Progress = 0;
                    }
                    View.Show(Progress, (ShowMenu) ? string.Empty : Notifications.ViewShown);
                    break;
                case Notifications.FadeAfterMenuSelect:
                    View.Show(Progress, Notifications.ViewShown);
                    break;
                case Notifications.ReturnToMenu:
                    View.Hide();
                    break;
            }
        }
    }
}
