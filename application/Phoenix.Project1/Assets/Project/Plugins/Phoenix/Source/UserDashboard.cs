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
    internal class UserDashboard : Regulus.Utility.IStatus, IDashboard
    {
        private readonly IBinder _Binder;
        private readonly IPlayer _Self;
        private readonly ILobby _Lobby;
        readonly INotifier<Phoenix.Project1.Common.Users.IActor> _Actors;

        
        readonly UniRx.CompositeDisposable _Disposables;
        public event System.Action BattleEvent;
        public event System.Action TeamEvent;
        public event System.Action HeroEvent;
        public event System.Action StoreEvent;

        public UserDashboard(IBinder binder, IPlayer self, ILobby lobby)
        {
            this._Binder = binder;
            this._Self = self;
            this._Lobby = lobby;
            _Actors = new Phoenix.Project1.GhostNotifier<IPlayer, Phoenix.Project1.Common.Users.IActor>(_Lobby.Players);
            _Disposables = new UniRx.CompositeDisposable();
        }
       /* private ICombat _GetFightFromRemote()
        {
            //todo: get fight service from remote
            return new Combat();
        }*/

        

        private void _UnbindActor(Phoenix.Project1.Common.Users.IActor actor)
        {
            _Binder.Unbind<Phoenix.Project1.Common.Users.IActor>(actor); 
        }

        private void _BindActor(Phoenix.Project1.Common.Users.IActor actor)
        {
            _Binder.Bind<Phoenix.Project1.Common.Users.IActor>(actor);
        }

        

       

        void IDashboard.RequestBattle()
        {
            BattleEvent();
        }

        void IStatus.Enter()
        {
            _Disposables.Add(_Actors.SupplyEvent().Subscribe(_BindActor));
            _Disposables.Add(_Actors.UnsupplyEvent().Subscribe(_UnbindActor));


            _Binder.Bind<IPlayer>(_Self);
            _Binder.Bind<IDashboard>(this);
        }

        void IStatus.Leave()
        {
            _Binder.Unbind<IDashboard>(this);
            _Binder.Unbind<IPlayer>(_Self);


            _Disposables.Clear();
        }

        void IStatus.Update()
        {
            
        }

        void IDashboard.RequestTeam()
        {
            TeamEvent();
        }

        void IDashboard.RequestHero()
        {
            HeroEvent();
        }

        void IDashboard.RequestStore()
        {
            StoreEvent();
        }
    }
}