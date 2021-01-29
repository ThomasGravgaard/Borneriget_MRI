using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class LobbyMediator : Mediator
    {
        public LobbyMediator() : base(NAME) {}

        public new static string NAME = "LobbyMediator";

        private LobbyView View => (LobbyView)ViewComponent;
        private PreferencesProxy Preferences;

        public static class Notifications
        {
            public const string StartNormalVideo = "StartNormalVideo";
            public const string StartVrVideo = "StartVrVideo";
        }

        public override void OnRegister()
        {
            base.OnRegister();
            Preferences = Facade.RetrieveProxy<PreferencesProxy>();
            InitializeView();
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<LobbyView>(true);
            View.SelectMode += View_SelectMode;
            View.Show(Preferences.Avatar);
        }

        private void View_SelectMode(bool isNormalMode)
        {
            Facade.SendNotification(isNormalMode ? Notifications.StartNormalVideo : Notifications.StartVrVideo);
            View.Hide();
        }
    }
}
