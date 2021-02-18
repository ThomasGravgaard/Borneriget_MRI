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
        private FadeNotification? SendOnFade = null;

        public static class Notifications
        {
            public const string StartFade = "StartFade";
            public const string FadeBlack = "FadeBlack";
        }

        public struct FadeNotification
        {
            public string Name;
            public object Body;
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
            if (SendOnFade.HasValue)
            {
                SendNotification(SendOnFade.Value.Name, SendOnFade.Value.Body);
                SendOnFade = null;
            }
            else
            {
                SendNotification(Notifications.FadeBlack);
            }
        }

        public override string[] ListNotificationInterests()
        {
            return new [] { Notifications.StartFade };
        }

        public override void HandleNotification(INotification notification)
        {
            Debug.Log($"Fader {notification.Name} {notification.Body}");
            if (notification.Name == Notifications.StartFade)
            {
                foreach (var view in Views)
                {
                    view.StartFade();
                }
                if (notification.Body != null)
                {
                    if (notification.Body is FadeNotification)
                    {
                        SendOnFade = (FadeNotification)notification.Body;
                    }
                    else
                    {
                        SendOnFade = new FadeNotification { Name = notification.Body.ToString(), Body = null };
                    }
                }
                else
                {
                    SendOnFade = null;
                }
            }
        }
    }
}
