using System;
using UnityEngine;

namespace Borneriget.MRI
{
    public class SoundView : MonoBehaviour
    {
        [SerializeField]
        private AudioClip buttonClick;
        [SerializeField]
        private AudioClip[] menuSpeaks;
        [SerializeField]
        private AudioClip[] menuSpeaks_UK;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void ClickButton()
        {
            audioSource.PlayOneShot(buttonClick);
        }

        public void MenuSpeak(int menuStep)
        {
            if (menuStep > -1 && menuStep < menuSpeaks.Length)
            {
                audioSource.PlayOneShot(menuSpeaks[menuStep]);
            }
        }

        public void MenuSpeakUK(int menuStep)
        {
            menuStep -= 2;
            if (menuStep > -1 && menuStep < menuSpeaks.Length)
            {
                audioSource.PlayOneShot(menuSpeaks_UK[menuStep]);
            }
        }
    }
}