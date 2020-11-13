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
        readonly Game.IConfigurationDatabase  _Configuration;
        public UserBattle(IBinder binder, Game.IConfigurationDatabase configuration)
        {
            _Configuration = configuration;
            _Binder = binder;
            _Battle = new Battles.Battle(_BuildDemoStage());
        }
        private Stage _BuildDemoStage()
        {
            
            var attacker = new Actor(1, 5, _Configuration);
            var defender = new Actor(2, 8, _Configuration);
            var aTeam = new Team(attacker);
            var dTeam = new Team(defender);
            var stage = new Stage(1, aTeam, dTeam);
            return stage;
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
