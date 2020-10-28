using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;

namespace Phoenix.Project1.Users
{
    internal class UserBattle : IBattle , Regulus.Utility.IBootable
    {
        private IBinder _Binder;
        private readonly ICombat _Combat;

        public event System.Action DoneEvent;
        public UserBattle(IBinder binder)
        {
            _Binder = binder;
            this._Combat = _GetFightFromRemote();


            

        }

        void IBattle.Exit()
        {
            DoneEvent();
        }

        public Value<BattleInfo> GetCampsByStageId(int stage_id)
        {
            return _Combat.GetCampsByStageId(stage_id);
        }

        public Value<BattleInfo> GetCampsByOpponentId(int opponent_id)
        {
            return _Combat.GetCampsByOpponentId(opponent_id);
        }

        public Value<BattleResult> ToFight(int battle_id)
        {
            return _Combat.ToFight(battle_id);
        }

        private ICombat _GetFightFromRemote()
        {
            //todo: get fight service from remote
            return new Game.Combat();
        }

        void IBootable.Launch()
        {
            _Binder.Bind<IBattle>(this);
        }

        void IBootable.Shutdown()
        {
            _Binder.Unbind<IBattle>(this);
        }

        
    }
}