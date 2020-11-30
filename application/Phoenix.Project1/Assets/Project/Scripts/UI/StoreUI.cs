using Phoenix.Project1.Client;
using Phoenix.Project1.Common;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class StoreUI : MonoBehaviour
{
    public GameObject ConfirmPanel;

    private readonly UniRx.CompositeDisposable _SendDisposables;

    public StoreUI()
    {
        _SendDisposables = new CompositeDisposable();
    }

    public void ToDashboard()
    {
        var teamObs = from team in NotifierRx.ToObservable().Supply<IStoreStatus>()
                      select team;

        teamObs.Subscribe(_ToDashboard).AddTo(_SendDisposables);
    }

    private void _ToDashboard(IStoreStatus team)
    {
        team.Exit();
    }

    public void Return(GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }

    public void Confirm()
    {
        ConfirmPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        _SendDisposables.Clear();
    }
}
