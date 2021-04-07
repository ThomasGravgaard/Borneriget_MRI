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

        private void PlayClip(AudioClip clip)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.clip = clip;
            audioSource.Play();
        }

        public void MenuSpeak(int menuStep)
        {
            PlayClip(menuSpeaks.SafeGet(menuStep));
        }

        public void MenuSpeakUK(int menuStep)
        {
            menuStep -= 2;
            PlayClip(menuSpeaks_UK.SafeGet(menuStep));
        }

        public void StopSpeak()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}