using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Borneriget.MRI
{
    public class FaderView : MonoBehaviour
    {
        public event Action OnFadeBlack;
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void StartFade()
        {
            animator.SetTrigger("Fade");
        }

        public void FadeProgress(string position)
        {
            if (position == "Black")
            {
                OnFadeBlack?.Invoke();
            }
        }
    }
}
