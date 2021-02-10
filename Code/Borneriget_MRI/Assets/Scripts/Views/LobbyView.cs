using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borneriget.MRI
{
    public class LobbyView : MonoBehaviour
    {
        [SerializeField]
        private GameObject Buttons;
        [SerializeField]
        private Button Room1;
        [SerializeField]
        private Button Room2;
        [SerializeField]
        private Camera MenuCam;

        public event Action<int> SelectRoom;

        private void Awake()
        {
            gameObject.SetActive(false);
            Buttons.SetActive(false);
            Room1.onClick.AddListener(Room1_Click);
            Room2.onClick.AddListener(Room2_Click);
        }

        public void Show()
        {
            MenuCam.enabled = true;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            MenuCam.enabled = false;
            gameObject.SetActive(false);
        }

        public void ShowButtons()
        {
            Buttons.SetActive(true);
        }

        private void Room1_Click()
        {
            SelectRoom?.Invoke(1);
            gameObject.SetActive(false);
        }

        private void Room2_Click()
        {
            SelectRoom?.Invoke(2);
            gameObject.SetActive(false);
        }
    }
}
