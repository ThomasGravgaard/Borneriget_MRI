using PureMVC.Interfaces;
using PureMVC.Patterns.Mediator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Borneriget.MRI
{
    public class StoryMediator : Mediator
    {
        public StoryMediator() : base(NAME) {}

        public new static string NAME = "StoryMediator";

        private IStoryView View => (IStoryView)ViewComponent;
        private IVideoControl VideoView => ViewComponent as IVideoControl;
        private PreferencesProxy Preferences;

        private bool AvatarAwake = false;
        private int Progress = 0;
        private bool ShowMenu = false;
        private bool Exiting = false;
        private bool OnMenu = true;

        public static class Notifications
        {
            public const string ViewInitialized = "ViewInitialized";
            public const string ViewShown = "ViewShown";
            public const string AvatarClicked = "AvatarClicked";
            public const string FadeAfterVideo = "FadeAfterVideo";
            public const string FadeAfterMenuSelect = "FadeAfterMenuSelect";
            public const string ShowPreferences = "ShowPreferences";
            public const string ShowMenu = "ShowMenu";
        }

        public override string[] ListNotificationInterests()
        {
            return new[] {
                Notifications.ViewInitialized,
                Notifications.ViewShown,
                Notifications.AvatarClicked,
                Notifications.FadeAfterVideo,
                Notifications.FadeAfterMenuSelect,
                Notifications.ShowPreferences,
                Notifications.ShowMenu,
                VideoMediator.Notifications.PlayVideo,
                VideoMediator.Notifications.VideoDone,
                VideoMediator.Notifications.VideoProgressUpdate,
                VideoMediator.Notifications.VideoPaused,
                VideoMediator.Notifications.VideoResumed,
                AvatarMediator.Notifications.SpeakDone, 
                AvatarMediator.Notifications.AvatarAwake
            };
        }

        public override void OnRegister()
        {
            base.OnRegister();
            Preferences = Facade.RetrieveProxy<PreferencesProxy>();

            Facade.RegisterMediator(new AvatarMediator(Preferences.Avatar, Preferences.Language == "da"));
            InitializeView();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.Exit -= View_Exit;
            View.SelectRoom -= View_SelectRoom;
            if (VideoView != null)
            {
                VideoView.StartSeek -= VideoView_StartSeek;
                VideoView.EndSeek -= VideoView_EndSeek;
                VideoView.SetSeekPosition -= VideoView_SetSeekPosition;
            }
        }

        private void InitializeView()
        {
            ViewComponent = Preferences.UseVr ? (IStoryView)Object.FindObjectOfType<Story3dView>(true) : (IStoryView)Object.FindObjectOfType<Story2dView>(true);
            View.Exit += View_Exit;
            View.SelectRoom += View_SelectRoom;
            View.Initialize(Notifications.ViewInitialized);
            if (VideoView != null)
            {
                VideoView.StartSeek += VideoView_StartSeek;
                VideoView.EndSeek += VideoView_EndSeek;
                VideoView.SetSeekPosition += VideoView_SetSeekPosition;
            }
        }

        private void VideoView_SetSeekPosition(float position)
        {
            SendNotification(VideoMediator.Notifications.VideoSeek, position);
        }

        private void VideoView_StartSeek()
        {
            SendNotification(VideoMediator.Notifications.StartVideoSeek);
        }

        private void VideoView_EndSeek()
        {
            SendNotification(VideoMediator.Notifications.EndVideoSeek);
        }

        private void View_Exit()
        {
            if (ShowMenu && OnMenu)
            {
                SendNotification(FaderMediator.Notifications.StartFade, Notifications.ShowPreferences);
            }
            else
            {
                Exiting = true;
                SendNotification(VideoMediator.Notifications.StopVideo);
                SendNotification(AvatarMediator.Notifications.StopSpeak);
                SendNotification(FaderMediator.Notifications.StartFade, Notifications.ShowMenu);
            }
        }

        private void View_SelectRoom(int index)
        {
            if (ShowMenu)
            {
                OnMenu = false;
                // We will start the speak and the video
                switch (index)
                {
                    case 0:
                        Progress = 1;
                        break;
                    case 1:
                        Progress = 2;
                        break;
                    case 2:
                        Progress = 4;
                        break;
                }
                SendNotification(FaderMediator.Notifications.StartFade, Notifications.FadeAfterMenuSelect);
            }
        }

        public override void HandleNotification(INotification notification)
        {
            switch (notification.Name)
            {
                case Notifications.ViewInitialized:
                    View.Show(Progress, Notifications.ViewShown, AvatarAwake);
                    break;
                case Notifications.ViewShown:
                    if (Progress < 4)
                    {
                        SendNotification(VideoMediator.Notifications.PrepareVideo, Progress);
                    }
                    else
                    {
                        Debug.Log("No video");
                    }
                    if (AvatarAwake)
                    {
                        if (Progress == 5)
                        {
                            SendNotification(AvatarMediator.Notifications.ShowDiploma);
                        }
                        SendNotification(AvatarMediator.Notifications.AvatarSpeak, Progress);
                    }
                    break;
                case Notifications.AvatarClicked:
                    if (!AvatarAwake)
                    {
                        SendNotification(AvatarMediator.Notifications.WakeAvatar);
                        AvatarAwake = true;
                    }
                    break;
                case AvatarMediator.Notifications.AvatarAwake:
                    SendNotification(AvatarMediator.Notifications.AvatarSpeak, Progress);
                    break;
                case AvatarMediator.Notifications.SpeakDone:
                    if (Progress < 4)
                    {
                        SendNotification(FaderMediator.Notifications.StartFade, new FaderMediator.FadeNotification
                        {
                            Name = VideoMediator.Notifications.PlayVideo,
                            Body = Progress
                        });
                    }
                    else
                    {
                        // We have no more videos, so just fade to the next speak.
                        SendNotification(AvatarMediator.Notifications.Idle);
                        SendNotification(FaderMediator.Notifications.StartFade, Notifications.FadeAfterVideo);
                    }
                    break;
                case VideoMediator.Notifications.PlayVideo:
                    View.ShowVideo();
                    break;
                case VideoMediator.Notifications.VideoDone:
                    if (!Exiting)
                    {
                        SendNotification(FaderMediator.Notifications.StartFade, Notifications.FadeAfterVideo);
                    }
                    break;
                case VideoMediator.Notifications.VideoProgressUpdate:
                    VideoView?.SetVideoProgress((VideoProgress)notification.Body);
                    break;
                case Notifications.FadeAfterVideo:
                    if (Progress++ == 5)
                    {
                        // We have shown all videos. We now have a selection menu.
                        ShowMenu = true;
                        Progress = 0;
                    }
                    OnMenu = true;
                    View.Show(Progress, (ShowMenu) ? string.Empty : Notifications.ViewShown, AvatarAwake);
                    break;
                case VideoMediator.Notifications.VideoPaused:
                    VideoView?.ShowPause();
                    break;
                case VideoMediator.Notifications.VideoResumed:
                    VideoView?.ShowResume();
                    break;
                case Notifications.FadeAfterMenuSelect:
                    View.Show(Progress, Notifications.ViewShown, AvatarAwake);
                    break;
                case Notifications.ShowPreferences:
                    View.Hide();
                    break;
                case Notifications.ShowMenu:
                    ShowMenu = true;
                    Progress = 0;
                    Exiting = false;
                    View.Show(Progress, string.Empty, AvatarAwake);
                    break;
            }
        }
    }
}
