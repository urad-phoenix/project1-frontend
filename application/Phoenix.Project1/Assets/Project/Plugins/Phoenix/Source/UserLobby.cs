using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Phoenix.Project1.Common.Users;
using Phoenix.Project1.Game;
using Regulus.Remote;
using Regulus.Remote.Reactive;
using Regulus.Utility;
using System;
using UniRx;

namespace Phoenix.Project1.Users
{
    internal class UserLobby : Regulus.Utility.IBootable, IBattle
    {
        private readonly IBinder _Binder;
        private readonly IPlayer _Self;
        private readonly ILobby _Lobby;
        private readonly IFight _Fight;
        readonly INotifier<IActor> _Actors;

        
        readonly UniRx.CompositeDisposable _Disposables;


        public UserLobby(IBinder binder, IPlayer self, ILobby lobby)
        {
            this._Binder = binder;
            this._Self = self;
            this._Lobby = lobby;
            this._Fight = _GetFightFromRemote();

            _Actors = new Phoenix.Project1.GhostNotifier<IPlayer,IActor>(_Lobby.Players);
            _Disposables = new UniRx.CompositeDisposable();
        }

        private IFight _GetFightFromRemote()
        {
            //todo: get fight service from remote
            return new Fight();
        }

        void IBootable.Launch()
        {

            _Disposables.Add(_Actors.SupplyEvent().Subscribe(_BindActor));
            _Disposables.Add(_Actors.UnsupplyEvent().Subscribe(_UnbindActor));


            _Binder.Bind<IPlayer>(_Self);
            _Binder.Bind<IBattle>(this);
        }

        private void _UnbindActor(IActor actor)
        {
            _Binder.Unbind<IActor>(actor); 
        }

        private void _BindActor(IActor actor)
        {
            _Binder.Bind<IActor>(actor);
        }

        void IBootable.Shutdown()
        {
            _Binder.Unbind<IPlayer>(_Self);
            _Binder.Unbind<IBattle>(this);

            _Disposables.Clear();
        }

        public Value<BattleResult> RequestBattleResult()
        {
            var result = _Fight.ToFight(123);
            return result;
        }
    }
}