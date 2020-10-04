using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Users;
using Regulus.Remote;
using Regulus.Remote.Reactive;
using Regulus.Utility;
using UniRx;

namespace Phoenix.Project1.Users
{
    internal class UserLobby : Regulus.Utility.IBootable
    {
        private readonly IBinder _Binder;
        private readonly IPlayer _Self;
        private readonly ILobby _Lobby;
        readonly INotifier<IActor> _Actors;

        
        readonly UniRx.CompositeDisposable _Disposables;


        public UserLobby(IBinder binder,IPlayer self,ILobby lobby)
        {
            _Binder = binder;
            this._Self = self;
            this._Lobby = lobby;
            
            _Actors = new Phoenix.Project1.GhostNotifier<IPlayer,IActor>(_Lobby.Players);
            _Disposables = new UniRx.CompositeDisposable();
        }

        void IBootable.Launch()
        {
            
            _Disposables.Add(_Actors.SupplyEvent().Subscribe(_Binder.Bind<IActor>));
            _Disposables.Add(_Actors.UnsupplyEvent().Subscribe(_Binder.Unbind<IActor>));


            _Binder.Bind<IPlayer>(_Self);
        }
        

        void IBootable.Shutdown()
        {
            _Binder.Unbind<IPlayer>(_Self);

            _Disposables.Dispose();
            _Disposables.Clear();
        }
    }
}