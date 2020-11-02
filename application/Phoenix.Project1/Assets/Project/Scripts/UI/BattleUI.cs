using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public class BattleUI : MonoBehaviour
    {
        public GameObject PauseMask;

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

        public void Pasue()
        {
            //using rx pause battle.
            PauseMask.SetActive(!PauseMask.activeSelf);
        }

        public void Return()
        {
            // using rx come back battle
            PauseMask.SetActive(!PauseMask.activeSelf);
            Debug.Log("Come back Battle.");
        }

        public void Skip()
        {
            //using rx get a immediate battle result.
        }

        public void AutoMasterSpell()
        {
            Debug.Log("AutoMasterSpell");
        }

        public void SpeedRate()
        {
            Debug.Log("SpeedRate");
        }

        public void MasterSpell(int id)
        {
            Debug.Log($"MasterSpell {id}");
        }
    }
}