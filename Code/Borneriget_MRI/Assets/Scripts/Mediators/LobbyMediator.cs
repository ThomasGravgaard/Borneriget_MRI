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
            public const string StartNormalVideo = "NormalVideo";
            public const string StartVrVideo = "VrVideo";
            public const string VideoDone = "VideoDone";
        }

        public override void OnRegister()
        {
            base.OnRegister();
            Preferences = Facade.RetrieveProxy<PreferencesProxy>();
            InitializeView();
        }

        public override string[] ListNotificationInterests()
        {
            return new[] { Notifications.VideoDone };
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<LobbyView>(true);
            View.SelectMode += View_SelectMode;
            View.Show(Preferences.Avatar);
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.VideoDone:
                    SceneManager.UnloadSceneAsync(VideoScene);
                    View.Show(Preferences.Avatar);
                    break;
            }
        }

        private void View_SelectMode(bool isNormalMode)
        {
            VideoScene = isNormalMode ? Notifications.StartNormalVideo : Notifications.StartVrVideo;
            Facade.SendNotification(VideoScene);
            SceneManager.LoadSceneAsync(VideoScene, LoadSceneMode.Additive);
            View.Hide();
        }
    }
}
