using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class SoundMediator : Mediator
    {
        public SoundMediator() : base(NAME) { }

        public new static string NAME = "SoundMediator";

        private SoundView View => (SoundView)ViewComponent;

        public static class Notifications
        {
            public const string ClickButton = "ClickButton";
            public const string MenuSpeak = "MenuSpeak";
            public const string StopSpeak = "StopSpeak";
        }

        public override void OnRegister()
        {
            InitializeView();
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<SoundView>(true);
        }

        public override string[] ListNotificationInterests()
        {
            return new[] { Notifications.ClickButton, Notifications.MenuSpeak, Notifications.StopSpeak };
        }

        public override void HandleNotification(INotification notification)
        {
            base.HandleNotification(notification);
            switch (notification.Name)
            {
                case Notifications.ClickButton:
                    View.ClickButton();
                    break;
                case Notifications.MenuSpeak:
                    var speak = (int)notification.Body;
                    var isDanish = true;
                    if (speak > 1)
                    {
                        // Check if we have UK prefs
                        var preferences = Facade.RetrieveProxy<PreferencesProxy>();
                        isDanish = !(preferences != null && preferences.Language == "en");
                    }
                    if (isDanish)
                    {
                        View.MenuSpeak(speak);
                    }
                    else
                    {
                        View.MenuSpeakUK(speak);
                    }
                    break;
                case Notifications.StopSpeak:
                    View.StopSpeak();
                    break;
            }
        }
    }
}
