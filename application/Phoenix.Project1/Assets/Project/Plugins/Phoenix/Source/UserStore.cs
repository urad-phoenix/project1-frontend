using Phoenix.Project1.Common;
using Regulus.Remote;

namespace Phoenix.Project1.Users
{
    internal class UserStore : IStoreStatus, Regulus.Utility.IStatus 
    {
        private IBinder _Binder;
        public event System.Action DoneEvent;

        public UserStore(IBinder binder)
        {
            _Binder = binder;
        }

        public void Enter()
        {
            _Binder.Bind<IStoreStatus>(this);
        }

        public void Leave()
        {
            _Binder.Unbind<IStoreStatus>(this);
        }

        public void Update()
        {
        }

        public void Exit()
        {
            DoneEvent();
        }
    }
        
}