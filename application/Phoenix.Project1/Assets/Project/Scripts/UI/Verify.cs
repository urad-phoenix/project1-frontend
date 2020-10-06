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
        public UnityEngine.UI.Toggle Standalone;
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
            Standalone.OnValueChangedAsObservable().Subscribe(_ => _Check()).AddTo(_UIDisposables);
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
            IObservable<bool> connectResult;
            if (Standalone.isOn)
            {
                connectResult = Agent.Instance.SetStandalone();
            }
            else
            {
                connectResult = Agent.Instance.SetTcp(_IpAddress , _Port);
            }

            _Verify(connectResult);


        }
        void _Verify(IObservable<bool> connect_obs)
        {
            _LoginDisposables.Clear();
            var obs = from _ in connect_obs
                      from msgBoxLogin in MessageBoxProvider.Instance.OpenObservable("登入中...", "提示")
                      from notifier in NotifierRx.ToObservable()
                      from verify in notifier.QueryNotifier<IVerifier>().SupplyEvent()
                      from result in _VerifyAccount(verify)                      
                      from closeDone in MessageBoxProvider.Instance.Close(msgBoxLogin)
                      select result;
            obs.Subscribe(_LoginResult).AddTo(_LoginDisposables);
            
        }

        System.IObservable<VerifyResult> _VerifyAccount(IVerifier verifier)
        {
            verifier.Verify(Account.text);
            return UniRx.Observable.Return(VerifyResult.Success);
        }

        private void _LoginResult(VerifyResult result)
        {
            if(result != VerifyResult.Success)
            {
                var msgBox = MessageBoxProvider.Instance.Open($"登入失敗:{result}","提示","確定");
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

            if (!Standalone.isOn && !_CheckIp())
            {
                return;
            }

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

