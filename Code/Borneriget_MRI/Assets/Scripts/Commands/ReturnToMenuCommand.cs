using PureMVC.Interfaces;

namespace Borneriget.MRI
{
    public class ReturnToMenuCommand : ChangeMediatorCommand<StoryMediator, PreferencesMediator>
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Facade.SendNotification(VideoMediator.Notifications.StopVideo);
            Facade.SendNotification(AvatarMediator.Notifications.StopSpeak);
        }
    }
}
