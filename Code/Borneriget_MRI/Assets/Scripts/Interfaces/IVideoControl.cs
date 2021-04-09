using System;

namespace Borneriget.MRI
{
    public interface IVideoControl
    {
        void ShowPause();
        void ShowResume();

        public event Action StartSeek;
        public event Action<float> SetSeekPosition;
        public event Action EndSeek;
    }
}