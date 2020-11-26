﻿using Phoenix.Project1.Battles;
using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using System.Linq;

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
            _Battle = new Battles.Battle(_BuildDemoStage() , binder);
        }
        private Stage _BuildDemoStage()
        {
            
            var attacker1 = new Actor(_Get(10002),1, 4);
            var attacker2 = new Actor(_Get(10002), 3,5);
            var attacker3 = new Actor(_Get(10001), 5,6);
            var attacker4 = new Actor(_Get(10002), 7, 1);
            var attacker5 = new Actor(_Get(10002), 9, 2);
            var attacker6 = new Actor(_Get(10002), 11, 3);
            var defender1 = new Actor(_Get(10002), 2,7);
            var defender2 = new Actor(_Get(10001), 4, 8);
            var defender3 = new Actor(_Get(10002), 6, 9);
            var defender4 = new Actor(_Get(10002), 8, 10);
            var defender5 = new Actor(_Get(10002), 10, 11);
            var defender6= new Actor(_Get(10002), 12, 12);
            //var aTeam = new Team(attacker1, attacker2, attacker3, attacker4, attacker5, attacker6);
            var aTeam = new Team(attacker1, attacker2, attacker3,attacker4, attacker5, attacker6);
            //var dTeam = new Team(defender1, defender2, defender3, defender4, defender5, defender6);
            var dTeam = new Team(defender1, defender2, defender3, defender4, defender5, defender6);
            var stage = new Stage(1, aTeam, dTeam);
            return stage;
        }

        private Configs.Actor _Get(int id)
        {
            return _Configuration.Query<Configs.Actor>().Where(a => a.Id == id).Single();
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
