using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class FaderMediator : Mediator
    {
        public FaderMediator() : base(NAME) { }

        public new static string NAME = "FaderMediator";

        private FaderView[] Views;
        private string SendOnFade = null;

        public static class Notifications
        {
            public const string StartFade = "StartFade";
            public const string FadeBlack = "FadeBlack";
        }

        public override void OnRegister()
        {
            Views = Object.FindObjectsOfType<FaderView>(true);
            Views[0].OnFadeBlack += FaderMediator_OnFadeBlack;
        }

        public override void OnRemove()
        {
            base.OnRemove();
            Views[0].OnFadeBlack -= FaderMediator_OnFadeBlack;
        }

        private void FaderMediator_OnFadeBlack()
        {
            SendNotification(SendOnFade ?? Notifications.FadeBlack);
        }

        public override string[] ListNotificationInterests()
        {
            return new [] { Notifications.StartFade };
        }

        public override void HandleNotification(INotification notification)
        {
            if (notification.Name == Notifications.StartFade)
            {
                foreach (var view in Views)
                {
                    view.StartFade();
                }
                SendOnFade = (notification.Body != null) ? notification.Body.ToString() : null; 
            }
        }
    }
}
