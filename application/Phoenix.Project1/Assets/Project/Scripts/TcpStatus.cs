﻿using Regulus.Remote.Client.Tcp;
using Regulus.Remote.Ghost;
using Regulus.Utility;
using System;
using System.Net;
using UniRx;

namespace Phoenix.Project1.Client
{
    internal class TcpStatus : IStatus 
    {
        private IPAddress _Ip;
        private int _Port;
        private IAgent _Agent
            ;
        private readonly Connecter _Conecter;
        private readonly IObservable<IOnlineable> _ConnectObs;
        readonly UniRx.CompositeDisposable _Disposables;
        IOnlineable _Onlineable;
        public readonly System.IObservable<bool> Result;
        public TcpStatus(IPAddress ip_address, int port, IAgent agent)
        {
            this._Ip = ip_address;
            this._Port = port;
            this._Agent = agent;
            _Disposables = new CompositeDisposable();
            _Conecter = new Regulus.Remote.Client.Tcp.Connecter(_Agent);
            _ConnectObs = _Conecter.Connect(new IPEndPoint(_Ip, _Port)).ToObservable().ObserveOnMainThread();
            

            Result = _ConnectObs.ContinueWith<IOnlineable , bool>(_ContinueWith );
        }

        private IObservable<bool> _ContinueWith(IOnlineable arg)
        {
            return UniRx.Observable.Return(arg != null);
        }

        void IStatus.Enter()
        {
            _ConnectObs.Subscribe(_Result).AddTo(_Disposables);
            
        }

        private void _Result(IOnlineable onlineable)
        {
            _Onlineable = onlineable;
        }

        void IStatus.Leave()
        {
            _Disposables.Clear();
        }

        void IStatus.Update()
        {
            _Agent.Update();
        }

 
    }
}