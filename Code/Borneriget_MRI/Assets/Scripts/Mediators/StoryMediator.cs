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
                VideoMediator.Notifications.VideoDone, 
                AvatarMediator.Notifications.SpeakDone, 
                AvatarMediator.Notifications.AvatarAwake 
            };
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<Story2dView>(true);
            View.Initialize(Notifications.ViewInitialized);
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
                    View.ShowVideo();
                    Facade.SendNotification(VideoMediator.Notifications.PlayVideo, Progress);
                    break;
                case VideoMediator.Notifications.VideoDone:
                    View.Show(Progress, Notifications.ViewShown);
                    break;
            }
        }
    }
}
