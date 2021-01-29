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

        public event Action<string> LanguageSelected;
        public event Action<string> AvatarSelected;

        private void Awake()
        {
            gameObject.SetActive(false);
            LanguageSelection.SetActive(false);
            AvatarSelection.SetActive(false);
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
            var target = eventData.pointerCurrentRaycast.gameObject;
            if (LanguageSelection.activeInHierarchy)
            {
                if (target == Danish)
                {
                    SelectLanguage("da-DK");
                }
                if (target == English)
                {
                    SelectLanguage("en-EN");
                }
            }
            else if (AvatarSelection.activeInHierarchy)
            {
                if (target == Theo)
                {
                    SelectAvatar("Theo");
                }
                if (target == Thea)
                {
                    SelectAvatar("Thea");
                }
            }
        }

        private void SelectLanguage(string language)
        {
            LanguageSelection.SetActive(false);
            LanguageSelected?.Invoke(language);
            AvatarSelection.SetActive(true);
        }

        private void SelectAvatar(string avatar)
        {
            AvatarSelection.SetActive(false);
            AvatarSelected?.Invoke(avatar);
        }
    }
}
