using Phoenix.Project1.Client;
using Phoenix.Project1.Common;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class HeroUI : MonoBehaviour
{
    private readonly UniRx.CompositeDisposable _SendDisposables;

    public HeroUI()
    {
        _SendDisposables = new CompositeDisposable();
    }

    public void ToDashboard()
    {
        var teamObs = from team in NotifierRx.ToObservable().Supply<IHeroStatus>()
                      select team;

        teamObs.Subscribe(_ToDashboard).AddTo(_SendDisposables);
    }

    private void _ToDashboard(IHeroStatus team)
    {
        team.Exit();
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
