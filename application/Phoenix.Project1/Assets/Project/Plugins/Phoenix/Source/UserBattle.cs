using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;

namespace Phoenix.Project1.Users
{
    internal class UserBattle : IBattle , Regulus.Utility.IBootable
    {
        private IBinder _Binder;
        private readonly IFight _Fight;

        public event System.Action DoneEvent;
        public UserBattle(IBinder binder)
        {
            _Binder = binder;
            this._Fight = _GetFightFromRemote();


            

        }

        void IBattle.Exit()
        {
            DoneEvent();
        }

        Value<BattleResult> IBattle.RequestBattleResult()
        {
            return _Fight.ToFight(0);
        }

        private IFight _GetFightFromRemote()
        {
            //todo: get fight service from remote
            return new Game.Fight();
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