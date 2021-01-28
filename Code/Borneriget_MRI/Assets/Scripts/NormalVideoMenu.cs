using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class NormalVideoMenu : MonoBehaviour
{
    [SerializeField]
    private Button StartButton;
    [SerializeField]
    private Button BackButton;
    [SerializeField]
    private GameObject MenuPanel;
    [SerializeField]
    private GameObject ButtonPanel;
    [SerializeField]
    private VideoPlayer Player;
    [SerializeField]
    private Image Progress;
    [SerializeField]
    private GameObject Video;


    void Awake()
    {
        ButtonPanel.SetActive(true);
        Video.SetActive(false);
        StartButton.onClick.AddListener(Start_OnClicked);
        BackButton.onClick.AddListener(Back_OnClicked);
    }

    private void Start_OnClicked()
    {
        StartCoroutine(PlayVideo());
    }

    private void Back_OnClicked()
    {
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu()
    {
        ButtonPanel.SetActive(false);
        yield return SceneManager.LoadSceneAsync("Main");
    }

    private IEnumerator PlayVideo()
    {
        MenuPanel.SetActive(false);
        Video.SetActive(true);
        Player.Play();
        //var totalTime = TimeSpan.FromSeconds(Player.frameCount / Player.frameRate);
        // Wait until player starts playing
        while (!Player.isPlaying)
        {
            yield return null;
        }
        while (Player.isPlaying)
        {
            // Update progress while playing
            var progress = ((float)Player.frame / (float)Player.frameCount);
            Progress.fillAmount = progress;
            //var currentTime = TimeSpan.FromSeconds(Player.frame / Player.frameRate);
            //ProgressLabel.text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
            yield return null;
        }
        Video.SetActive(false);
        MenuPanel.SetActive(true);
    }
}
