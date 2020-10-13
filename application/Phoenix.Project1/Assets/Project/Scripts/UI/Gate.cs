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

        private Verify _Verify;

        private Lobby _Lobby;

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

            
            NotifierRx.ToObservable().Supply<IVerifier>().Subscribe(_ShowVerify).AddTo(_Disposables);
            NotifierRx.ToObservable().Unsupply<IVerifier>().Subscribe(_HideVerify).AddTo(_Disposables);

            NotifierRx.ToObservable().Supply<IPlayer>().Subscribe(_ShowLobby).AddTo(_Disposables);
            NotifierRx.ToObservable().Unsupply<IPlayer>().Subscribe(_HideLobby).AddTo(_Disposables);

            _ShowVerify(null);
        }

        private void _HideLobby(IPlayer obj)
        {
            _LobbyFadeOut();            
        }

        private void _ShowLobby(IPlayer obj)
        {
            if(_Lobby)
            {
                _LobbyFadeIn();

                return;
            }

            var go = Instantiate(Lobby);

            _Lobby = go.GetComponent<Lobby>();

            if(_Lobby)
            {
                _Lobby.transform.SetTransformParent(this.transform);

                _LobbyFadeIn();
            }
        }

        private void _LobbyFadeIn()
        {
            var observable = _Lobby.transform.FadeInAsObserver();

            observable.Subscribe(_ =>
            {
                _Lobby.Show();
            });
        }

        private void _LobbyFadeOut()
        {
            var observable = _Lobby.transform.FadeOutAsObserver();

            observable.Subscribe(_ =>
            {
                _Lobby.Hide();
            });

        }

        private void _HideVerify(IVerifier obj)
        {
            _VerifyFadeOut();
        }

        private void _ShowVerify(IVerifier obj)
        {
            if(_Verify)
            {
                _VerifyFadeIn();
                return;
            }

            var go = Instantiate(Verify);

            _Verify = go.GetComponent<Verify>();

            if(_Verify)
            {
                _Verify.transform.SetTransformParent(this.transform);

                _VerifyFadeIn();
            }            
        }


        private void _VerifyFadeIn()
        {
            var observable = _Verify.transform.FadeInAsObserver();

            observable.Subscribe(_ =>
            {
                _Verify.Show();
            });
        }

        private void _VerifyFadeOut()
        {
            var observable = _Verify.transform.FadeOutAsObserver();

            observable.Subscribe(_ =>
            {
                _Verify.Hide();
            });

        }

    }

}
