using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borneriget.MRI
{

    public class PreferencesView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject LanguageSelection;
        [SerializeField]
        private GameObject Danish;
        [SerializeField]
        private GameObject English;
        [SerializeField]
        private GameObject AvatarSelection;
        [SerializeField]
        private GameObject Theo;
        [SerializeField]
        private GameObject Thea;
        [SerializeField]
        private GameObject FormatSelection;
        [SerializeField]
        private GameObject Tablet;
        [SerializeField]
        private GameObject Cardboard;
        [SerializeField]
        private GameObject Frame;

        public event Action<string> LanguageSelected;
        public event Action<PreferencesProxy.Avatars> AvatarSelected;
        public event Action<bool> FormatSelected;

        private void Awake()
        {
            Frame.SetActive(false);
            LanguageSelection.SetActive(false);
            AvatarSelection.SetActive(false);
            FormatSelection.SetActive(false);
        }

        public void Show(bool hasSelectedLanguage)
        {
            gameObject.SetActive(true);
            Frame.SetActive(true);
            if (hasSelectedLanguage)
            {
                Bootstrap.Facade.SendNotification(SoundMediator.Notifications.MenuSpeak, 2);
                FormatSelection.SetActive(true);
            }
            else
            {
                Bootstrap.Facade.SendNotification(SoundMediator.Notifications.MenuSpeak, 1);
                LanguageSelection.SetActive(true);
            }
        }

        public void Hide()
        {
            Frame.SetActive(false);
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Frame.activeInHierarchy)
            {
                return;
            }
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (LanguageSelection.activeInHierarchy)
            {
                if (target == Danish)
                {
                    SelectLanguage("da");
                }
                if (target == English)
                {
                    SelectLanguage("en");
                }
            }
            else if (AvatarSelection.activeInHierarchy)
            {
                if (target == Theo)
                {
                    SelectAvatar(PreferencesProxy.Avatars.Theo);
                }
                if (target == Thea)
                {
                    SelectAvatar(PreferencesProxy.Avatars.Thea);
                }
            }
            else if (FormatSelection.activeInHierarchy)
            {
                if (target == Tablet)
                {
                    SelectFormat(false);
                }
                if (target == Cardboard)
                {
                    SelectFormat(true);
                }
            }
        }

        private void SelectLanguage(string language)
        {
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
            LanguageSelection.SetActive(false);
            LanguageSelected?.Invoke(language);
            AvatarSelection.SetActive(true);
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.MenuSpeak, 2);
        }

        private void SelectAvatar(PreferencesProxy.Avatars avatar)
        {
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
            AvatarSelected?.Invoke(avatar);
            AvatarSelection.SetActive(false);
            FormatSelection.SetActive(true);
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.MenuSpeak, 3);
        }

        private void SelectFormat(bool useVr)
        {
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
            FormatSelection.SetActive(false);
            FormatSelected?.Invoke(useVr);
        }
    }
}
