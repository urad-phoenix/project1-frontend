
using UnityEngine;
using UniRx;
using Phoenix.Project1.Common;
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
            var disconnectObs = from agent in Agent.ToObservable()
                                from change in agent.ObserveEveryValueChanged(a => a.Active)
                                where change == false
                                select agent;
            disconnectObs.Subscribe(_ToLogin).AddTo(_Disposables);

            var playerObs = from player in NotifierRx.ToObservable().Supply<IDashboard>()
                            select player;
            playerObs.Subscribe(_ToDashboard).AddTo(_Disposables);

            

        }

        private void _ToDashboard(IDashboard player)
        {
            _Loader.OpenDashboard();
        }

        private void _ToLogin(object obj)
        {
            _Loader.OpenLogin();
        }

        private void OnDestroy()
        {
            _Disposables.Clear();
            _Machine.Clean();
            _Loader.Close();
        }
    }

}
