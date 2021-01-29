using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using UnityEngine.SceneManagement;

namespace Borneriget.MRI
{
    public class ShowNormalVideo : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            SceneManager.LoadScene("NormalVideo");
        }
    }

}
