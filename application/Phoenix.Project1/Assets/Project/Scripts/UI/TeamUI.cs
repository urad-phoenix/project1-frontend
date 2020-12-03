using Phoenix.Project1.Common;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public class TeamUI : MonoBehaviour
    {
        public GameObject MasterSpellPreview;

        private readonly UniRx.CompositeDisposable _SendDisposables;

        public TeamUI()
        {
            _SendDisposables = new CompositeDisposable();
        }

        public void ToDashboard()
        {
            var teamObs = from team in NotifierRx.ToObservable().Supply<ITeamStatus>()
                            select team;

            teamObs.ObserveOnMainThread().Subscribe(_ToDashboard).AddTo(_SendDisposables);
        }

        private void _ToDashboard(ITeamStatus team)
        {
            team.Exit();
        }

        public void ShowMasterSpellPreview()
        {
            MasterSpellPreview.SetActive(true);
        }

        public void Return(GameObject go)
        {
            go.SetActive(!go.activeSelf);
        }

        private void OnDestroy()
        {
            _SendDisposables.Clear();
        }
    }
}