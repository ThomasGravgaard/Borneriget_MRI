using PureMVC.Patterns.Mediator;
using System;

namespace Borneriget.MRI
{
    public class TitleScreenMediator : Mediator
    {
        public TitleScreenMediator() : base(NAME) { }

        public new static string NAME = "TitleScreenMediator";

        private TitleScreenView View => (TitleScreenView)ViewComponent;

        public static class Notifications
        {
            public const string TitleScreenShown = "TitleScreenShown";
        }

        public override void OnRegister()
        {
            base.OnRegister();

            InitializeView();
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.MenuSpeak, 0);
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.TimerDone -= View_TimerDone;
            View.Hide();
        }

        private void InitializeView()
        {
            ViewComponent = UnityEngine.Object.FindObjectOfType<TitleScreenView>(true);
            View.TimerDone += View_TimerDone;
            View.Show();
        }

        private void View_TimerDone()
        {
            Facade.SendNotification(Notifications.TitleScreenShown);
        }
    }
}
