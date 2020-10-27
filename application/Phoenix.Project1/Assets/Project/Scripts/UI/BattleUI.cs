using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public class BattleUI : MonoBehaviour
    {
        private readonly UniRx.CompositeDisposable _SendDisposables;
        private readonly UniRx.CompositeDisposable _SendRequestDisposables;

        public BattleUI()
        {
            _SendDisposables = new CompositeDisposable();
            _SendRequestDisposables = new CompositeDisposable();
        }

        private void Start()
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattle>()
                select battle;

            battleObs.Subscribe(_RequestBattle).AddTo(_SendRequestDisposables);
        }

        private void _RequestBattle(IBattle battle)
        {
           var values =  battle.RequestBattleResult();
           var results = values.GetValue();
            Debug.Log(results);
        }

        public void ToDashboard()
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattle>()
                select battle;
            
            battleObs.Subscribe(_ToDashboard).AddTo(_SendDisposables); 
        }

        private void OnDestroy()
        {
            _SendDisposables.Clear();
            _SendRequestDisposables.Clear();
        }

        private void _ToDashboard(IBattle battle)
        {
            battle.Exit();
        }    
    }
}