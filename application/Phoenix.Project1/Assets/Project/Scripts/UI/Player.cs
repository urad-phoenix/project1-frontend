using Phoenix.Project1.Client;
using Phoenix.Project1.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;
namespace Phoenix.Project1.Client.UI
{ 
    public class Player : MonoBehaviour
    {
        public UnityEngine.GameObject Root;
        public UnityEngine.GameObject Income;
        public UnityEngine.GameObject StagePreview;

        //public UnityEngine.UI.Text Message;
        //public UnityEngine.UI.Button Send;
        readonly UniRx.CompositeDisposable _Disposables;
        //readonly UniRx.CompositeDisposable _SendDisposables;
        readonly UniRx.CompositeDisposable _SendBattleDisposables;
        readonly UniRx.CompositeDisposable _SendTeamDisposables;
        public Player()
        {
            _Disposables = new CompositeDisposable();
            //_SendDisposables = new CompositeDisposable();
            _SendBattleDisposables = new CompositeDisposable();
            _SendTeamDisposables = new CompositeDisposable();
        }
        // Start is called before the first frame update
        void Start()
        {
            NotifierRx.ToObservable().Supply<IPlayer>().Subscribe(_Show).AddTo(_Disposables);
            NotifierRx.ToObservable().Unsupply<IPlayer>().Subscribe(_Hide).AddTo(_Disposables);
            
        }
       
        private void OnDestroy()
        {
            _Disposables.Clear();
            //_SendDisposables.Clear();
            _SendBattleDisposables.Clear();
            _SendTeamDisposables.Clear();
        }

        private void _Hide(IPlayer obj)
        {
            Root.SetActive(false);
        }

        private void _Show(IPlayer player)
        {
            Root.SetActive(true);
            //_SendDisposables.Clear();
            //Send.OnClickAsObservable().Subscribe(_=> _Send(player) ).AddTo(_SendDisposables);
        }

        public void ToStagePreview()
        {
            // using rx get stage info.
            StagePreview.SetActive(true);
        }

        public void ToIncome()
        {
            Income.SetActive(true);
        }

        public void Return(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }

        //private void _Send(IPlayer player)
        //{
        //    player.SetMessage(Message.text);
        //}

        public void ToBattle()
        {
            var dashObs = from dash in NotifierRx.ToObservable().Supply<IDashboard>()
                          select dash;

            dashObs.Subscribe(_Tobattl).AddTo(_SendBattleDisposables);
        }

        private void _Tobattl(IDashboard dash)
        {
            dash.RequestBattle();
        }
        
        public void ToTeam()
        {
            var dashObs = from dash in NotifierRx.ToObservable().Supply<IDashboard>()
                          select dash;

            dashObs.Subscribe(_ToTeam).AddTo(_SendTeamDisposables);
        }

        private void _ToTeam(IDashboard dash)
        {
            dash.RequestTeam();
        }

        public void ToHero()
        {
            var dashObs = from dash in NotifierRx.ToObservable().Supply<IDashboard>()
                          select dash;

            dashObs.Subscribe(_ToHero).AddTo(_SendTeamDisposables);
        }

        private void _ToHero(IDashboard dash)
        {
            dash.RequestHero();
        }

        public void ToStore()
        {
            var dashObs = from dash in NotifierRx.ToObservable().Supply<IDashboard>()
                          select dash;

            dashObs.Subscribe(_ToStore).AddTo(_SendTeamDisposables);
        }

        private void _ToStore(IDashboard dash)
        {
            dash.RequestStore();
        }
    }

}