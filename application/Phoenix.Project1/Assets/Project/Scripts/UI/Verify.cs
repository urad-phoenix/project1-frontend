using Phoenix.Project1.Common.Users;

using System.Net;
using UnityEngine;
using UniRx;
using Regulus.Remote.Reactive;
using System;
using System.Collections;
using Regulus.Remote;

namespace Phoenix.Project1.Client.UI
{
    public class Verify : MonoBehaviour
    {
        public UnityEngine.UI.Button Login;
        public UnityEngine.UI.InputField Account;
        public UnityEngine.UI.InputField IpAddress;
        public UnityEngine.UI.InputField Port;
        IPAddress _IpAddress;
        private int _Port;
        readonly UniRx.CompositeDisposable _UIDisposables;
        readonly UniRx.CompositeDisposable _LoginDisposables;
        public Verify()
        {
            _UIDisposables = new UniRx.CompositeDisposable();
            _LoginDisposables = new CompositeDisposable();
        }
        public void Start()
        {
            Account.OnValueChangedAsObservable().Subscribe(_ => _Check()).AddTo(_UIDisposables);
            IpAddress.OnValueChangedAsObservable().Subscribe(_ => _Check()).AddTo(_UIDisposables);
            Port.OnValueChangedAsObservable().Subscribe(_ => _Check()).AddTo(_UIDisposables);
            Login.OnClickAsObservable().Subscribe(_ => _Login() ).AddTo(_UIDisposables);

            
        }

        internal void Show()
        {
            gameObject.SetActive(true);
        }

        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        private void _Login()
        {
            
            _Verify();


        }
        void _Verify()
        {
            _LoginDisposables.Clear();


            var connectObs = 
                             from notifier in NotifierRx.ToObservable()
                             from connecter in notifier.QueryNotifier<IConnecter>().SupplyEvent()
                             from msgBoxConnect in MessageBoxProvider.Instance.OpenObservable("提示", "連線中...")
                             from result in connecter.Connect(_GetIpAddress()).RemoteValue().ObserveOnMainThread()
                             from _ in MessageBoxProvider.Instance.Close(msgBoxConnect)
                             select result;

            connectObs.DoOnError(_Error).Subscribe(_ConnectResult).AddTo(_LoginDisposables);


            var loginObs =
                    
                    from notifier in NotifierRx.ToObservable()
                    from verifier in notifier.QueryNotifier<IVerifier>().SupplyEvent()
                    from msgBoxVerify in MessageBoxProvider.Instance.OpenObservable("提示", "驗證中...")
                    from result in verifier.Verify(Account.text).RemoteValue()
                    from _ in MessageBoxProvider.Instance.Close(msgBoxVerify)

                    select result;
            loginObs.DefaultIfEmpty(VerifyResult.Fail).Subscribe(_LoginResult).AddTo(_LoginDisposables);

        }

        private void _Error(Exception obj)
        {
            throw new NotImplementedException();
        }

        private void _ConnectResult(bool result)
        {
            if (!result )
            {
                var msgBox = MessageBoxProvider.Instance.Open("提示", $"連線失敗", "確定");
                msgBox.Buttons[0].OnClickAsObservable().Subscribe(_ => MessageBoxProvider.Instance.Close(msgBox)).AddTo(_LoginDisposables);
            }
        }

        private IPEndPoint _GetIpAddress()
        {
            return new IPEndPoint(_IpAddress, _Port);
        }

        

        private void _LoginResult(VerifyResult result)
        {
            if(result != VerifyResult.Success)
            {
                var msgBox = MessageBoxProvider.Instance.Open("提示", $"登入失敗:{result}", "確定");
                msgBox.Buttons[0].OnClickAsObservable().Subscribe(_ => MessageBoxProvider.Instance.Close(msgBox)).AddTo(_LoginDisposables);
            }
        }

        void _Check()
        {
            Login.interactable = false;
            if (string.IsNullOrWhiteSpace(Account.text))
            {
                return;
            }
            if (!_CheckIp())
                return;

            Login.interactable = true;
        }

        private bool _CheckIp()
        {
            if (!System.Net.IPAddress.TryParse(IpAddress.text, out _IpAddress))
            {
                return false ;
            }


            if (!int.TryParse(Port.text, out _Port))
            {
                return false;
            }
            return true;
        }

        public void OnDestroy()
        {
            _UIDisposables.Clear();
        }
    }

}

