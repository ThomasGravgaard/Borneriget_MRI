using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Borneriget.MRI
{
    public class LobbyMediator : Mediator
    {
        public LobbyMediator() : base(NAME) {}

        public new static string NAME = "LobbyMediator";

        private LobbyView View => (LobbyView)ViewComponent;
        private PreferencesProxy Preferences;
        private string VideoScene = "";

        public static class Notifications
        {
            public const string SpeakDone = "SpeakDone";
            public const string StartNormalVideo = "NormalVideo";
            public const string StartVrVideo = "VrVideo";
            public const string VideoDone = "VideoDone";
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
            return new[] { Notifications.SpeakDone, Notifications.VideoDone };
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<LobbyView>(true);
            View.SelectRoom += View_SelectMode;
            View.Show();
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.VideoDone:
                    SceneManager.UnloadSceneAsync(VideoScene);
                    View.Show();
                    break;
                case Notifications.SpeakDone:
                    View.ShowButtons();
                    break;
            }
        }

        private void View_SelectMode(int room)
        {
            VideoScene = Preferences.UseVr ? Notifications.StartVrVideo : Notifications.StartNormalVideo;
            Facade.SendNotification(VideoScene);
            SceneManager.LoadSceneAsync(VideoScene, LoadSceneMode.Additive);
            View.Hide();
        }
    }
}
