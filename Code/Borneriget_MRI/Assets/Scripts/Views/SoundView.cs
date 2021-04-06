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
    }
}