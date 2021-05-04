using System;
using UnityEngine;
using UnityEngine.UI;

namespace Borneriget.MRI
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField]
        private Button[] Buttons;

        [SerializeField]
        private string[] menuTexts_DK;
        [SerializeField]
        private string[] menuTexts_UK;

        public event Action<int> MenuSelected;

        private void Awake()
        {
            var buttonIndex = 0;
            foreach (var button in Buttons)
            {
                var idx = buttonIndex++;
                button.onClick.AddListener(() => MenuSelected?.Invoke(idx));
            }
        }

        public void Initialize(bool isDanish)
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                var label = Buttons[i].GetComponentInChildren<Text>(true);
                label.text = (isDanish) ? menuTexts_DK[i] : menuTexts_UK[i];
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            foreach (var button in Buttons)
            {
                button.interactable = true;
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            foreach (var button in Buttons)
            {
                button.interactable = false;
            }
        }

        public string GetText(int room)
        {
            var button = -1;
            if (room == 1)
            {
                button = 0;
            }
            if (room == 2)
            {
                button = 1;
            }
            if (room == 4)
            {
                button = 2;
            }
            if (button == -1)
            {
                return string.Empty;
            }
            return Buttons[button].GetComponentInChildren<Text>(true).text;
        }
    }
}
