using System;

namespace Borneriget.MRI
{
    public interface IVideoControl
    {
        public void SetVideoProgress(VideoProgress progress);
        void ShowPause();
        void ShowResume();

        public event Action StartSeek;
        public event Action<float> SetSeekPosition;
        public event Action EndSeek;
    }
}