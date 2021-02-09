using PureMVC.Patterns.Mediator;
using UnityEngine;

namespace Borneriget.MRI
{
    public class AvatarMediator : Mediator
    {
        public AvatarMediator(PreferencesProxy.Avatars avatar) : base(NAME) 
        {
            Avatar = avatar;
        }

        public new static string NAME = "AvatarMediator";

        private AvatarView View => (AvatarView)ViewComponent;

        private PreferencesProxy.Avatars Avatar;
        public override void OnRegister()
        {
            base.OnRegister();
            InitializeView();
        }

        private void InitializeView()
        {
            ViewComponent = Object.FindObjectOfType<AvatarView>(true);
            View.Show(Avatar);
        }
    }
}
