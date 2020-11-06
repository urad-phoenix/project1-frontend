using Phoenix.Project1.Common;
using Regulus.Remote;

namespace Phoenix.Project1.Users
{
    internal class UserHero : IHeroStatus, Regulus.Utility.IStatus
    {
        private IBinder _Binder;
        public event System.Action DoneEvent;

        public UserHero(IBinder binder)
        {
            _Binder = binder;
        }

        public void Enter()
        {
            _Binder.Bind<IHeroStatus>(this);
        }

        public void Leave()
        {
            _Binder.Unbind<IHeroStatus>(this);
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