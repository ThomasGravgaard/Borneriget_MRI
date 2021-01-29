using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Borneriget.MRI
{
    public class LobbyView : MonoBehaviour
    {
        [SerializeField]
        private Button NormalButton;
        [SerializeField]
        private Button VrButton;
        [SerializeField]
        private GameObject Theo;
        [SerializeField]
        private GameObject Thea;

        public event Action<bool> SelectMode;

        private void Awake()
        {
            Debug.Log("lobby awake");
            gameObject.SetActive(false);
            NormalButton.onClick.AddListener(NormalButton_Click);
            VrButton.onClick.AddListener(VrButton_Click);
        }

        public void Show(string avatar)
        {
            Debug.Log("Show lobby");
            gameObject.SetActive(true);
            Theo.SetActive(avatar == "Theo");
            Thea.SetActive(avatar == "Thea");
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void VrButton_Click()
        {
            SelectMode?.Invoke(false);
            gameObject.SetActive(false);
        }

        private void NormalButton_Click()
        {
            SelectMode?.Invoke(true);
            gameObject.SetActive(false);
        }
    }
}
