using System;
using System.Collections;
using UnityEngine;

namespace Borneriget.MRI
{
    public class TitleScreenView : MonoBehaviour
    {
        [SerializeField]
        private float showTimer = 3f;

        public event Action TimerDone;

        public void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(ViewCo());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator ViewCo()
        {
            yield return new WaitForSeconds(showTimer);
            TimerDone();
        }
    }
}
