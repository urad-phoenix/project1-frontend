using Phoenix.Project1.Common;
using Regulus.Remote;

namespace Phoenix.Project1.Users
{
    internal class UserTeam :ITeamStatus, Regulus.Utility.IStatus
    {
        private IBinder _Binder;
        public event System.Action DoneEvent;

        public UserTeam(IBinder binder)
        {
            _Binder = binder;
        }

        public void Enter()
        {
            _Binder.Bind<ITeamStatus>(this);
        }

        public void Leave()
        {
            _Binder.Unbind<ITeamStatus>(this);
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