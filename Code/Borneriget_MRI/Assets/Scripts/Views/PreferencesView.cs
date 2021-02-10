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

        public event Action<string> LanguageSelected;
        public event Action<PreferencesProxy.Avatars> AvatarSelected;
        public event Action<bool> FormatSelected;

        private AudioSource audioSource;

        private void Awake()
        {
            gameObject.SetActive(false);
            LanguageSelection.SetActive(false);
            AvatarSelection.SetActive(false);
            FormatSelection.SetActive(false);
            audioSource = GetComponent<AudioSource>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            LanguageSelection.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var selected = false;
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (LanguageSelection.activeInHierarchy)
            {
                if (target == Danish)
                {
                    SelectLanguage("da-DK");
                    selected = true;
                }
                if (target == English)
                {
                    SelectLanguage("en-EN");
                    selected = true;
                }
            }
            else if (AvatarSelection.activeInHierarchy)
            {
                if (target == Theo)
                {
                    SelectAvatar(PreferencesProxy.Avatars.Theo);
                    selected = true;
                }
                if (target == Thea)
                {
                    SelectAvatar(PreferencesProxy.Avatars.Thea);
                    selected = true;
                }
            }
            else if (FormatSelection.activeInHierarchy)
            {
                if (target == Tablet)
                {
                    SelectFormat(false);
                    selected = true;
                }
                if (target == Cardboard)
                {
                    SelectFormat(true);
                    selected = true;
                }
            }
            if (selected)
            {
                audioSource.Play();
            }
        }

        private void SelectLanguage(string language)
        {
            LanguageSelection.SetActive(false);
            LanguageSelected?.Invoke(language);
            AvatarSelection.SetActive(true);
        }

        private void SelectAvatar(PreferencesProxy.Avatars avatar)
        {
            AvatarSelection.SetActive(false);
            AvatarSelected?.Invoke(avatar);
            FormatSelection.SetActive(true);
        }

        private void SelectFormat(bool useVr)
        {
            FormatSelection.SetActive(false);
            FormatSelected?.Invoke(useVr);
        }
    }
}
