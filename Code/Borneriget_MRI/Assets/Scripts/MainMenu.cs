using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI Label;
    [SerializeField]
    private Button NormalButton;
    [SerializeField]
    private Button VRButton;
    [SerializeField]
    private GameObject ButtonPanel;

    private void Awake()
    {
        NormalButton.onClick.AddListener(NormalButton_Click);
        VRButton.onClick.AddListener(VRButton_Click);
    }

    private void VRButton_Click()
    {
        StartCoroutine(LoadScene("VrVideo"));
    }

    private void NormalButton_Click()
    {
        StartCoroutine(LoadScene("NormalVideo"));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        ButtonPanel.SetActive(false);
        Label.text = "Loading";
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}
