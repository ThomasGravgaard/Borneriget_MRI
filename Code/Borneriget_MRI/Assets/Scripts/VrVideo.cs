using Borneriget.MRI;
using Google.XR.Cardboard;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.XR.Management;

public class VrVideo : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer Player;
    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private TMPro.TextMeshProUGUI Label;
    [SerializeField]
    private GameObject Environment;
    [SerializeField]
    private GameObject GazeTarget;
    [SerializeField]
    private Camera Cam;
    [SerializeField]
    private float UiDistance = 1;
    [SerializeField]
    private Image Progress;
    [SerializeField]
    private float ProgressSpeed = 1.5f;
    [SerializeField]
    private float MouseRotateSpeed = 45f;

    private float UiYoffset;
    private float progress;
    private Vector3 lastMousePos;
    private Transform camRoot;

    private bool vrPlaying = false;

    // Start is called before the first frame update
    void Start()
    {
        UiYoffset = transform.position.y - Cam.transform.position.y;
        GazeTarget.SetActive(false);
        StartCoroutine(InitializeXR());
        StartCoroutine(PrepareVideo());
        camRoot = Cam.transform;
        while (camRoot.parent)
        {
            camRoot = camRoot.parent;
        }
#if UNITY_EDITOR
        lastMousePos = Input.mousePosition;
#endif
    }

    private void ButtonClicked()
    {
        if (vrPlaying)
        {
            StopXR();
        }
        else
        {
            StartCoroutine(StartVideo());
        }
    }

    private void Log(string message, bool error = false)
    {
        //Label.text = message;
        //Label.color = (error) ? Color.red : Color.white;
        if (error)
        {
            Debug.LogError(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    private IEnumerator InitializeXR()
    {
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Log("Initializing XR Failed.");
        }
        else
        {
            if (!Api.HasDeviceParams())
            {
                Api.ScanDeviceParams();
            }
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Log("XR started.");
            vrPlaying = true;
            yield return new WaitForSeconds(0.1f);
            GazeTarget.SetActive(true);
            RepositionTarget();
        }
    }

    private void RepositionTarget()
    {
        // Move the target within field of view
        var vec = GazeTarget.transform.position - Cam.transform.position;
        var dist = vec.magnitude;
        vec.Normalize();
        vec = new Vector3(Cam.transform.forward.x, vec.y, Cam.transform.forward.z);
        GazeTarget.transform.position = Cam.transform.position + (vec * dist);
    }

    private IEnumerator PrepareVideo()
    {
        var videoProxy = Bootstrap.Facade.RetrieveProxy<VideoProxy>();
        Player.url = videoProxy.GetVrVideo();
        Player.Prepare();
        while (!Player.isPrepared)
        {
            yield return null;
        }
    }

    private IEnumerator StartVideo()
    {
        Environment.SetActive(false);
        UI.SetActive(false);
        while (!Player.isPrepared)
        {
            yield return null;
        }
        Player.Play();
        while (!Player.isPlaying)
        {
            yield return null;
        }
        TimeSpan? totalTime = null;
        float? progress = null;
        while (Player.isPlaying)
        {
            try
            {
                if (!totalTime.HasValue && Player.frameRate > 0)
                {
                    totalTime = TimeSpan.FromSeconds(Player.frameCount / Player.frameRate);
                }
                if (Player.frameCount > 0)
                {
                    progress = ((float)Player.frame / (float)Player.frameCount);
                }
                if (Player.frameRate > 0)
                {
                    var currentTime = TimeSpan.FromSeconds(Player.frame / Player.frameRate);
                    Label.text = $"{currentTime:mm\\:ss} / {totalTime:mm\\:ss}";
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Uncaught exception. {ex.Message}");
            }
            yield return null;
        }
        Bootstrap.Facade.SendNotification(LobbyMediator.Notifications.VideoDone);
    }

    private void StopXR()
    {
        if (Player.isPlaying)
        {
            Player.Stop();
        }

        Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        Log("XR stopped.");

        Log("Deinitializing XR...");
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Log("XR deinitialized.");

        Bootstrap.Facade.SendNotification(LobbyMediator.Notifications.VideoDone);
    }

    public void Update()
    {
        if (vrPlaying)
        {
            if (Api.IsCloseButtonPressed)
            {
                StopXR();
            }

            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }

            Api.UpdateScreenParams();

            if (GazeTarget.activeInHierarchy)
            {
                // See if we are looking at the target
                var progressDelta = 0f;
                if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out RaycastHit hit))
                {
                    if (hit.transform.gameObject == GazeTarget)
                    {
                        progressDelta = (1 / ProgressSpeed) * Time.deltaTime;
                    }
                }
                if (progressDelta > 0)
                {
                    progress = Mathf.Clamp01(progress + progressDelta);
                }
                else
                {
                    progress = 0;
                }
                Progress.fillAmount = progress;
                if (progress == 1)
                {
                    progress = 0;
                    StartCoroutine(StartVideo());
                }

                // Rotate the target
                GazeTarget.transform.Rotate(new Vector3(0, 90 * Time.deltaTime, 0));

                // Position UI in front of camera
                transform.position = (Cam.transform.position + Cam.transform.forward * UiDistance) + new Vector3(0, UiYoffset, 0);
                transform.LookAt(Cam.transform);
                transform.forward = -transform.forward;

            }
#if UNITY_EDITOR
            var mouseDelta = lastMousePos - Input.mousePosition;
            lastMousePos = Input.mousePosition;
            var rotate = new Vector3(mouseDelta.y, mouseDelta.x, 0) * Time.deltaTime * MouseRotateSpeed;
            camRoot.transform.Rotate(rotate);
#endif
        }
    }
}
