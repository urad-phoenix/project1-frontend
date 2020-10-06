using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Users;
using Regulus.Extension;
using Regulus.Remote.Reactive;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
namespace Phoenix.Project1.Client.UI
{
    public class Gate : MonoBehaviour
    {
        public Phoenix.Project1.Client.UI.Verify Verify;
        public Phoenix.Project1.Client.UI.Lobby Lobby;
        readonly UniRx.CompositeDisposable _Disposables;
        public Gate()
        {
            _Disposables = new UniRx.CompositeDisposable();

        }
        private void OnDestroy()
        {
            _Disposables.Clear();
        }
        void Start()
        {

            ;
            NotifierRx.ToObservable().Supply<IVerifier>().Subscribe(_ShowVerify).AddTo(_Disposables);
            NotifierRx.ToObservable().Unsupply<IVerifier>().Subscribe(_HideVerify).AddTo(_Disposables);

            NotifierRx.ToObservable().Supply<IPlayer>().Subscribe(_ShowLobby).AddTo(_Disposables);
            NotifierRx.ToObservable().Unsupply<IPlayer>().Subscribe(_HideLobby).AddTo(_Disposables);
        }

        private void _HideLobby(IPlayer obj)
        {
            Lobby.Hide();
        }

        private void _ShowLobby(IPlayer obj)
        {
            Lobby.Show();
        }

        private void _HideVerify(IVerifier obj)
        {
            Verify.Hide();
        }

        private void _ShowVerify(IVerifier obj)
        {
            Verify.Show();
        }

        
    }

}
