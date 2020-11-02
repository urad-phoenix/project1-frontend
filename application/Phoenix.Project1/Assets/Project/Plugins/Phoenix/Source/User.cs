using Phoenix.Project1.Common;
using Regulus.Remote;
using System;
using System.Collections.Generic;

namespace Phoenix.Project1.Users
{
    class User : System.IDisposable
    {
        private readonly IBinder _Binder;
        readonly Regulus.Utility.StageMachine _Machine;
        private readonly ILobby _Lobby;

        public User(IBinder binder,ILobby lobby)
        {
            
            _Lobby = lobby;
            this._Binder = binder;
            _Machine = new Regulus.Utility.StageMachine();

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

        void IDisposable.Dispose()
        {
            _Machine.Clean();
        }
    }
}
