using System;
using UnityEngine;

namespace Borneriget.MRI
{
    public class SoundView : MonoBehaviour
    {
        [SerializeField]
        private AudioClip buttonClick;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void ClickButton()
        {
            audioSource.PlayOneShot(buttonClick);
        }
    }
}