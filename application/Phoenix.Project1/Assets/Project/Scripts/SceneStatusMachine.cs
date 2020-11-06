﻿
using UnityEngine;
using UniRx;
using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Phoenix.Project1.Common.Users;

namespace Phoenix.Project1.Client
{
    public class SceneStatusMachine : MonoBehaviour
    {
        readonly SceneLoader _Loader;
        readonly Regulus.Utility.StageMachine _Machine;
        readonly UniRx.CompositeDisposable _Disposables;
        public SceneStatusMachine()
        {
            _Loader = new SceneLoader();
            _Disposables = new CompositeDisposable();
            _Machine = new Regulus.Utility.StageMachine();
        }
        void Start()
        {
            NotifierRx.ToObservable().Supply<IVerifier>().Subscribe(_ToLogin).AddTo(_Disposables);
            NotifierRx.ToObservable().Supply<IConnecter>().Subscribe(_ToLogin).AddTo(_Disposables);

            var playerObs = from player in NotifierRx.ToObservable().Supply<IDashboard>()
                            select player;
            
            playerObs.Subscribe(_ToDashboard).AddTo(_Disposables);

            var battleObs = from player in NotifierRx.ToObservable().Supply<IBattleStatus>()
                select player;
            
            battleObs.Subscribe(_ToBattle).AddTo(_Disposables);

            var teamObs = from player in NotifierRx.ToObservable().Supply<ITeamStatus>()
                            select player;

            teamObs.Subscribe(_ToTeam).AddTo(_Disposables);

            var storeObs = from player in NotifierRx.ToObservable().Supply<IStoreStatus>()
                          select player;

            storeObs.Subscribe(_ToStore).AddTo(_Disposables);

            var heroObs = from player in NotifierRx.ToObservable().Supply<IHeroStatus>()
                          select player;

            heroObs.Subscribe(_ToHero).AddTo(_Disposables);
        }

        private void _ToDashboard(IDashboard player)
        {
            _Loader.OpenDashboard();
        }

        private void _ToLogin(object obj)
        {
            _Loader.OpenLogin();
        }


        private void _ToBattle(IBattleStatus player)
        {
            _Loader.OpenBattle();
        }

        private void _ToTeam(ITeamStatus player)
        {
            _Loader.OpenTeam();
        }

        private void _ToHero(IHeroStatus player)
        {
            _Loader.OpenHero();
        }

        private void _ToStore(IStoreStatus player)
        {
            _Loader.OpenStore();
        }

        private void OnDestroy()
        {
            _Disposables.Clear();
            _Machine.Clean();
            _Loader.Close();
        }
    }

}
