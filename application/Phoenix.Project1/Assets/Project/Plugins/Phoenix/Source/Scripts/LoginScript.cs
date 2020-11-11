using Phoenix.Project1.Common.Users;
using Regulus.Remote;
using Regulus.Remote.Reactive;
using System.Net;
using UniRx;

namespace Phoenix.Project1.Client.Scripts
{
    internal class LoginScript : IScriptable
    {
        private readonly INotifierQueryable _Notifier;
        private readonly IPEndPoint _Ip;
        private readonly string _Account;
        readonly CompositeDisposable _Disposables;

        public LoginScript(INotifierQueryable notifier,IPEndPoint ip, string account)
        {
            _Ip = ip;
            _Account = account;
            this._Notifier = notifier;
            _Disposables = new CompositeDisposable();
        }

        void IScriptable.End()
        {
            
        }

        void IScriptable.Start()
        {
            var connectObs =    from connecter in _Notifier.NotifierSupply<IConnecter>()
                                from connectResult in connecter.Connect(_Ip).RemoteValue()
                                select connectResult;
            connectObs.Subscribe().AddTo(_Disposables);

            var verifyObs = from verify in _Notifier.NotifierSupply<IVerifier>()
                            from verifyResult in verify.Verify(_Account).RemoteValue()
                            select verifyResult;
            verifyObs.Subscribe().AddTo(_Disposables);

        }
    }
}