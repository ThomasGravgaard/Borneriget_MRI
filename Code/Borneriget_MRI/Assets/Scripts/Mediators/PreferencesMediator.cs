using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borneriget.MRI
{
    public class PreferencesMediator : Mediator
    {
		public PreferencesMediator() : base(NAME)
		{
		}

		public new static string NAME = "PreferencesMediator";

        private PreferencesView View => (PreferencesView)ViewComponent;

        public override void OnRegister()
        {
            base.OnRegister();
            InitializeView();
        }

        private void InitializeView()
        {
            // The preferences menu should already be present in the scene. Just get it.
            ViewComponent = Object.FindObjectOfType<PreferencesView>(true);
            View.Show();
        }
    }
}
