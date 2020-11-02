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