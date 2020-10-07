using Phoenix.Project1.Common.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
namespace Phoenix.Project1.Client.UI
{


    public class Actor : MonoBehaviour
    {
        IActor _Actor;

        public UnityEngine.UI.Text Account;
        public UnityEngine.UI.Text Message;
        public Guid Id => _Actor.Id;

        readonly UniRx.CompositeDisposable _Disposables;
        public Actor()
        {
            _Disposables = new CompositeDisposable();
        }
        private void OnDestroy()
        {
            _Disposables.Clear();
        }
        internal void Setup(IActor actor)
        {
            _Disposables.Clear();

            _Actor = actor;
            Account.text = _Actor.Account.Value;
            _Actor.Message.ObserveEveryValueChanged(m => m.Value).Subscribe(_ChangeMessage).AddTo(_Disposables);
        }

        private void _ChangeMessage(string message)
        {
            Message.text = message;
        }
    }
}