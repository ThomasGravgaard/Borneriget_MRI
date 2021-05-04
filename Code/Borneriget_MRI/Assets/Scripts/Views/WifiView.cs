using System;
using UnityEngine;

namespace Borneriget.MRI
{
    public class WifiView : MonoBehaviour
    {
        [SerializeField]
        public CanvasGroup canvasGroup;
        [SerializeField]
        public string text_DK;
        [SerializeField]
        public string text_UK;

        public void Show()
        {
            canvasGroup.alpha = 1;
        }
    }
}
