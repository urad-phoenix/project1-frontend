using Phoenix.Project1.Battles;
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
            
            var attacker1 = new Actor(_Get(10001),1, 1);
            var attacker2 = new Actor(_Get(10001), 2, 2);
            var attacker3 = new Actor(_Get(10001), 3, 3);
            var attacker4 = new Actor(_Get(10001), 4, 4);
            var attacker5 = new Actor(_Get(10001), 5, 5);
            var attacker6 = new Actor(_Get(10001), 6, 6);
            var defender1 = new Actor(_Get(10001), 7,7);
            var defender2 = new Actor(_Get(10001), 8, 8);
            var defender3 = new Actor(_Get(10001), 9, 9);
            var defender4 = new Actor(_Get(10001), 10, 10);
            var defender5 = new Actor(_Get(10001), 11, 11);
            var defender6= new Actor(_Get(10001), 12, 12);
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
