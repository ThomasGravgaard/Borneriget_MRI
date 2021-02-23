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
            Player.targetTexture = (isVr) ? VrTexture : NormalTexture;
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

        public void TogglePause()
        {
            if (Player.isPlaying)
            {
                PausedVideo = true;
                Player.Pause();
            }
            else
            {
                PausedVideo = false;
                Player.Play();
            }
        }

        public void StopVideo()
        {
            if (Player.isPlaying)
            {
                Player.Stop();
            }
        }
    }
}
