using Phoenix.Project1.Battles;
using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using System;

namespace Phoenix.Project1.Battles
{
}
namespace Phoenix.Project1.Users
{

    internal class UserBattle : Regulus.Utility.IStatus , IBattleStatus
    {
        private readonly IBinder _Binder;
        readonly Battles.Battle _Battle;

        public event System.Action DoneEvent;
        public UserBattle(IBinder binder)
        {
            _Binder = binder;
            _Battle = new Battles.Battle(_BuildDemoStage());
        }
        private Stage _BuildDemoStage()
        {
            return Stage.GetDemo();
        }

        void IStatus.Enter()
        {

            _Battle.Start();
            _Binder.Bind<IBattleStatus>(this);
            _Binder.Bind<IBattle>(_Battle);
        }

        void IStatus.Leave()
        {
            _Binder.Unbind<IBattle>(_Battle);
            _Binder.Unbind<IBattleStatus>(this);
            _Battle.End();
        }

        void IStatus.Update()
        {
            _Battle.Step();
        }

        void IBattleStatus.Exit()
        {
            DoneEvent();
        }
    }
}
