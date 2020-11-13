using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;

namespace Phoenix.Project1.Battles
{
    internal class BattleReady : Regulus.Utility.IStatus , Common.Battles.IReady
    {
        private bool _Ready;
        private readonly IBinder binder;

        public event System.Action DoneEvent;
        public BattleReady(Regulus.Remote.IBinder binder)
        {
            this.binder = binder;
        }

        void IStatus.Enter()
        {
            binder.Bind<IReady>(this);


        }

        void IStatus.Leave()
        {
            binder.Unbind<IReady>(this);
        }

        void IStatus.Update()
        {
            if(_Ready)
            {
                DoneEvent();
            }
        }

        void IReady.Ready()
        {
            _Ready = true;
        }
    }
}