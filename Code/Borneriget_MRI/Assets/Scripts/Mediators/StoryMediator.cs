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

        public static class Notifications
        {
            public const string ViewInitialized = "ViewInitialized";
            public const string ViewShown = "ViewShown";
            public const string AvatarClicked = "AvatarClicked";
            public const string FadeAfterVideo = "FadeAfterVideo";
            public const string ReturnToMenu = "ReturnToMenu";
        }

        public override void OnRegister()
        {
            base.OnRegister();
            Preferences = Facade.RetrieveProxy<PreferencesProxy>();

            Facade.RegisterMediator(new AvatarMediator(Preferences.Avatar));
            InitializeView();
        }

        public override string[] ListNotificationInterests()
        {
            return new[] {
                Notifications.ViewInitialized,
                Notifications.ViewShown,
                Notifications.AvatarClicked,
                Notifications.FadeAfterVideo,
                Notifications.ReturnToMenu,
                VideoMediator.Notifications.PlayVideo,
                VideoMediator.Notifications.VideoDone, 
                VideoMediator.Notifications.VideoProgressUpdate,
                AvatarMediator.Notifications.SpeakDone, 
                AvatarMediator.Notifications.AvatarAwake
            };
        }

        private void InitializeView()
        {
            Debug.Log("Initialize story");
            ViewComponent = Preferences.UseVr ? (IStoryView)Object.FindObjectOfType<Story3dView>(true) : (IStoryView)Object.FindObjectOfType<Story2dView>(true);
            View.Exit += View_Exit;
            View.Initialize(Notifications.ViewInitialized);
        }

        private void View_Exit()
        {
            SendNotification(FaderMediator.Notifications.StartFade, Notifications.ReturnToMenu);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            Debug.Log("Remove story");
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.ViewInitialized:
                    View.Show(Progress, Notifications.ViewShown);
                    break;
                case Notifications.ViewShown:
                    Facade.SendNotification(VideoMediator.Notifications.PrepareVideo, Progress);
                    if (AvatarAwake)
                    {
                        Facade.SendNotification(AvatarMediator.Notifications.AvatarSpeak, Progress);
                    }
                    break;
                case Notifications.AvatarClicked:
                    if (!AvatarAwake)
                    {
                        Facade.SendNotification(AvatarMediator.Notifications.WakeAvatar);
                        AvatarAwake = true;
                    }
                    break;
                case AvatarMediator.Notifications.AvatarAwake:
                    Facade.SendNotification(AvatarMediator.Notifications.AvatarSpeak, Progress);
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
                    Progress++;
                    View.Show(Progress, Notifications.ViewShown);
                    break;
                case Notifications.ReturnToMenu:
                    View.Hide();
                    break;
            }
        }
    }
}
