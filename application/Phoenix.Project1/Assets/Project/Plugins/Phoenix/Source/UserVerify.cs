using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Users;
using Regulus.Remote;
using Regulus.Remote.Reactive;
using Regulus.Utility;
using System;
using UniRx;

namespace Phoenix.Project1.Users
{

    internal class UserVerify : Regulus.Utility.IStatus, IVerifier
    {
        private readonly IBinder _Binder;
        private readonly ILobby _Lobby;
        private IDisposable _LoadHandler;

        public event System.Action<IPlayer> DoneEvent;
        public UserVerify(IBinder binder, ILobby lobby)
        {
            _Binder = binder;
            this._Lobby = lobby;
            DoneEvent += (p) => { };
        }

        

       

        Value<VerifyResult> IVerifier.Verify(string account)
        {
            if (_LoadHandler != null)
                return VerifyResult.Busy;
            var ret = new Regulus.Remote.Value<VerifyResult>();
            var obs = from playerId in _Lobby.LoadPlayer(account).RemoteValue()
                        where playerId != Guid.Empty
                        from player in _Lobby.Players.SupplyEvent()
                        where player.Id.Value == playerId
                        select player;
            _LoadHandler =  obs.DefaultIfEmpty(null).Subscribe(player => _VerifyResult(player , ret));
            return ret;
        }

        private void _VerifyResult(IPlayer player, Value<VerifyResult> ret)
        {
            _LoadHandler = null;

            if (player != null)
            {

                ret.SetValue(VerifyResult.Success);
                DoneEvent(player);
            }
            else
            {
                ret.SetValue(VerifyResult.Fail);
            }
        }

        void IStatus.Enter()
        {
            _Binder.Bind<IVerifier>(this);
        }

        void IStatus.Leave()
        {
            _Binder.Unbind<IVerifier>(this);
            if (_LoadHandler != null)
                _LoadHandler.Dispose();
        }

        void IStatus.Update()
        {
            
        }
    }
}

