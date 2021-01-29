using PureMVC.Interfaces;
using PureMVC.Patterns.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Borneriget.MRI
{
    /// <summary>
    /// This command will remove one mediator and register another to advance the flow.
    /// </summary>
    public class ChangeMediatorCommand<T1, T2> : SimpleCommand 
        where T1 : IMediator 
        where T2 : IMediator
    {
        public override void Execute(INotification notification)
        {
            base.Execute(notification);
            Facade.RemoveMediator<T1>();
            Facade.RegisterMediator(Activator.CreateInstance<T2>());
        }
    }
}
