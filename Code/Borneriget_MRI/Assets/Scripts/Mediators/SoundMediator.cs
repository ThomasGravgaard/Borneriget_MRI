﻿using PureMVC.Interfaces;
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
            return new[] { Notifications.ClickButton };
        }

        public override void HandleNotification(INotification notification)
        {
            base.HandleNotification(notification);
            if (notification.Name == Notifications.ClickButton)
            {
                View.ClickButton();
            }
        }
    }
}