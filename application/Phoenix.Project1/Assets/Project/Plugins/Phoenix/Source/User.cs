using Phoenix.Project1.Common;
using Regulus.Remote;
using Regulus.Utility;
using System;
using System.Collections.Generic;

namespace Phoenix.Project1.Users
{
    class User : Regulus.Utility.IUpdatable
    {
        private readonly IBinder _Binder;
        readonly Regulus.Utility.StatusMachine _Machine;
        private readonly ILobby _Lobby;
        bool _Enable;
        public User(IBinder binder,ILobby lobby)
        {
            
            _Lobby = lobby;
            this._Binder = binder;
            _Machine = new Regulus.Utility.StatusMachine();
            _Enable = true;
            binder.BreakEvent += ()=>_Enable = false;
            _ToVerify();
        }

        private void _ToVerify()
        {
            var stage = new UserVerify(_Binder,_Lobby);
            stage.DoneEvent += _ToDashboard;
            _Machine.Push(stage);
        }

        private void _ToDashboard(IPlayer player)
        {
            var stage = new UserDashboard(_Binder, player,_Lobby);
            stage.BattleEvent += ()=> _ToBattle(player);
            _Machine.Push(stage);

        }

        private void _ToBattle(IPlayer player)
        {
            var stage = new UserBattle(_Binder);
            stage.DoneEvent += ()=> _ToDashboard(player);
            _Machine.Push(stage);
        }

        
        bool IUpdatable.Update()
        {
            _Machine.Update();

            return _Enable;
        }

        void IBootable.Launch()
        {
            
        }

        void IBootable.Shutdown()
        {
            _Machine.Termination();
        }
    }
}
