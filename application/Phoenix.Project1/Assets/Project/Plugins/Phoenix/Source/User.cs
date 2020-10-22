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
            stage.DoneEvent += _ToLobby;
            _Machine.Push(stage);
        }

        private void _ToLobby(IPlayer player)
        {
            var stage = new UserLobby(_Binder, player,_Lobby);            
            _Machine.Push(stage);

        }

        void IDisposable.Dispose()
        {
            _Machine.Clean();
        }
    }
}
