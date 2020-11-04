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
    internal class UserDashboard : Regulus.Utility.IBootable  , IDashboard
    {
        private readonly IBinder _Binder;
        private readonly IPlayer _Self;
        private readonly ILobby _Lobby;
        private readonly ICombat _Fight;
        readonly INotifier<IActor> _Actors;

        
        readonly UniRx.CompositeDisposable _Disposables;
        public event System.Action BattleEvent;

        public UserDashboard(IBinder binder, IPlayer self, ILobby lobby)
        {
            this._Binder = binder;
            this._Self = self;
            this._Lobby = lobby;
            _Actors = new Phoenix.Project1.GhostNotifier<IPlayer,IActor>(_Lobby.Players);
            _Disposables = new UniRx.CompositeDisposable();
        }
        private ICombat _GetFightFromRemote()
        {
            //todo: get fight service from remote
            return new Combat();
        }

        void IBootable.Launch()
        {

            _Disposables.Add(_Actors.SupplyEvent().Subscribe(_BindActor));
            _Disposables.Add(_Actors.UnsupplyEvent().Subscribe(_UnbindActor));

            
            _Binder.Bind<IPlayer>(_Self);
            _Binder.Bind<IDashboard>(this);
            
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
            _Binder.Unbind<IDashboard>(this);
            _Binder.Unbind<IPlayer>(_Self);


            _Disposables.Clear();
        }

        void IDashboard.RequestBattle()
        {
            BattleEvent();
        }

        
    }
}