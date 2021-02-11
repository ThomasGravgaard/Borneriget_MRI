using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class AvatarMediator : Mediator
    {
        public AvatarMediator(PreferencesProxy.Avatars avatar) : base(NAME) 
        {
            Avatar = avatar;
        }

        public new static string NAME = "AvatarMediator";

        public static class Notifications
        {
            public const string WakeAvatar = "WakeAvatar";
            public const string AvatarAwake = "AvatarAwake";
            public const string AvatarSpeak = "AvatarSpeak";
            public const string SpeakDone = "SpeakDone";
        }

        private AvatarView View => (AvatarView)ViewComponent;

        private PreferencesProxy.Avatars Avatar;
        public override void OnRegister()
        {
            base.OnRegister();
            InitializeView();
        }

        public override string[] ListNotificationInterests()
        {
            return new[] { 
                Notifications.WakeAvatar,
                Notifications.AvatarSpeak,
            };
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<AvatarView>(true);
            View.Show(Avatar);
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.WakeAvatar:
                    View.WakeUp(() => Facade.SendNotification(Notifications.AvatarAwake));
                    break;
                case Notifications.AvatarSpeak:
                    View.Speak(() => Facade.SendNotification(Notifications.SpeakDone));
                    break;
            }
        }
    }
}
