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
            
            var attacker = new Actor(_Get(1001),1, 5);
            var defender = new Actor(_Get(1001), 2,8);
            var aTeam = new Team(attacker);
            var dTeam = new Team(defender);
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
