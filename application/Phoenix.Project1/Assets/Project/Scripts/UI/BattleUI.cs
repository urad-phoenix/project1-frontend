using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

namespace Phoenix.Project1.Client.UI
{
    public class BattleUI : MonoBehaviour
    {        
        private readonly UniRx.CompositeDisposable _SendDisposables;               

        private IBattle _Battle;     

        public void _ShowBattle(IBattle battle)
        {
            _Battle = battle;
        }       
        
        private void _Send()
        {            
            _Battle.Exit();
        }
        
        public void ToBattle()
        {
            var battleObs = from dash in NotifierRx.ToObservable().Supply<IBattle>()
                select dash;
            
            battleObs.Subscribe(_ToDashboard).AddTo(_SendDisposables);
        }

        private void _ToDashboard(IBattle battle)
        {
            battle.Exit();
        }    
    }
}