using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

namespace Borneriget.MRI
{
    public class VideoView : MonoBehaviour
    {
        [SerializeField]
        private VideoPlayer Player;
        [SerializeField]
        private RenderTexture NormalTexture;
        [SerializeField]
        private RenderTexture VrTexture;
        [SerializeField]
        private bool ShortenInEditor;

        public event Action VideoPrepared;
        public event Action VideoDone;
        public event Action<VideoProgress> VideoProgressUpdate;

        private bool PausedVideo = false;

        public void Initialize(bool isVr)
        {
            Player.errorReceived += Player_errorReceived;
            Player.targetTexture = (isVr) ? VrTexture : NormalTexture;
        }

        private void Player_errorReceived(VideoPlayer source, string message)
        {
            Debug.LogError($"Player error {message}. Url: {source.url}");
        }

        public void Prepare(string videoUrl)
        {
            Player.url = videoUrl;
            StartCoroutine(PrepareVideoCo());
        }

        private IEnumerator PrepareVideoCo()
        {
            Player.Prepare();
            while (!Player.isPrepared)
            {
                yield return null;
            }
            VideoPrepared?.Invoke();
        }

        public void PlayVideo()
        {
            StartCoroutine(PlayVideoCo());
        }

        private IEnumerator PlayVideoCo()
        {
            while (!Player.isPrepared)
            {
                yield return null;
            }
            Player.Play();           
            var totalTime = (Player.frameRate > 0) ? TimeSpan.FromSeconds(Player.frameCount / Player.frameRate) : TimeSpan.FromSeconds(0);
            var lastProgress = 0f;
            while (Player.isPlaying || PausedVideo)
            {
                // Update progress while playing
                if (Player.frameCount > 0)
                {
                    var progress = ((float)Player.frame / (float)Player.frameCount);
                    var currentTime = (Player.frameRate > 0) ? TimeSpan.FromSeconds(Player.frame / Player.frameRate) : TimeSpan.FromSeconds(0);
                    VideoProgressUpdate?.Invoke(new VideoProgress
                    {
                        TotalTime = totalTime,
                        CurrentTime = currentTime,
                        Progress = progress
                    });

                    if (ShortenInEditor && Application.isEditor && progress > 0.1f)
                    {
                        Player.Stop();
                        break;
                    }

                    lastProgress = progress;
                }

                yield return null;
            }
            VideoDone?.Invoke();
        }

        public bool TogglePause()
        {
            if (Player.isPlaying)
            {
                PausedVideo = true;
                Player.Pause();
                return true;
            }
            else
            {
                PausedVideo = false;
                Player.Play();
                return false;
            }
        }

        public void StopVideo()
        {
            if (Player.isPlaying)
            {
                Player.Stop();
            }
        }

        public void SeekTo(float percentage)
        {
            if (Player.frameRate > 0 && Player.canSetTime)
            {
                var totalTime = Player.frameCount / Player.frameRate;
                Player.time = totalTime * Mathf.Clamp01(percentage);
            }
            else
            {
                Debug.LogError($"Seek not possible!!! {Player.canSetTime}");
            }
        }

        public void StartSeek()
        {
            PausedVideo = true;
            Player.Pause();
        }

        public void EndSeek()
        {
            PausedVideo = false;
            Player.Play();
        }
    }
}
