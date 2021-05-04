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
        private GameObject Phone;
        [SerializeField]
        private GameObject Cardboard;
        [SerializeField]
        private GameObject Frame;
        [SerializeField]
        private float RepeatSpeakTimer = 20;
        [SerializeField]
        private WifiView WifiMessage;

        public event Action<bool> DanishSelected;
        public event Action<PreferencesProxy.Avatars> AvatarSelected;
        public event Action<bool> FormatSelected;
        private bool HasFormatBeenSelected = false;

        private Coroutine SpeakRoutine;

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
            HasFormatBeenSelected = false;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                WifiMessage.Show();
                return;
            }

            if (hasSelectedLanguage)
            {
                SpeakRoutine = StartCoroutine(SpeakRepeat(2));
                FormatSelection.SetActive(true);
            }
            else
            {
                SpeakRoutine = StartCoroutine(SpeakRepeat(1));
                FormatSelection.SetActive(false);
                LanguageSelection.SetActive(true);
            }
        }

        private void StopSpeak()
        {
            if (SpeakRoutine != null)
            {
                StopCoroutine(SpeakRoutine);
                SpeakRoutine = null;
            }
        }

        private IEnumerator SpeakRepeat(int speakIndex)
        {
            StopSpeak();
            while (true)
            {
                Bootstrap.Facade.SendNotification(SoundMediator.Notifications.MenuSpeak, speakIndex);
                yield return new WaitForSeconds(RepeatSpeakTimer);
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
                    SelectLanguage(true);
                }
                if (target == English)
                {
                    SelectLanguage(false);
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
            else if (FormatSelection.activeInHierarchy && !HasFormatBeenSelected)
            {
                if (target == Tablet || target == Phone)
                {
                    SelectFormat(false);
                }
                if (target == Cardboard)
                {
                    SelectFormat(true);
                }
            }
        }

        private void SelectLanguage(bool danishSelected)
        {
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
            LanguageSelection.SetActive(false);
            DanishSelected?.Invoke(danishSelected);
            AvatarSelection.SetActive(true);
            SpeakRoutine = StartCoroutine(SpeakRepeat(2));
        }

        private void SelectAvatar(PreferencesProxy.Avatars avatar)
        {
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
            AvatarSelected?.Invoke(avatar);
            AvatarSelection.SetActive(false);
            FormatSelection.SetActive(true);
            SpeakRoutine = StartCoroutine(SpeakRepeat(3));
        }

        private void SelectFormat(bool useVr)
        {
            HasFormatBeenSelected = true;
            StopSpeak();
            Bootstrap.Facade.SendNotification(SoundMediator.Notifications.ClickButton);
            FormatSelected?.Invoke(useVr);
        }
    }
}
