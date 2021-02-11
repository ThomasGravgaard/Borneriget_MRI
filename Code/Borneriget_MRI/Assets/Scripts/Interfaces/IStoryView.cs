using System;

namespace Borneriget.MRI
{
    public interface IStoryView
    {
        public void Initialize(string doneNotification);
        public void Show(int room, string doneNotification);
        public void ShowVideo();
        public void Hide();
    }
}