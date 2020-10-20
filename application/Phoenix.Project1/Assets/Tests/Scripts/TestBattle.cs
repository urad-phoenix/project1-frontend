using Phoenix.Project1.Client;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote.Reactive;
using UniRx;
using UnityEngine;

public class TestBattle : MonoBehaviour
{
    public UnityEngine.GameObject Root;
    public UnityEngine.UI.Button Battle;
    readonly UniRx.CompositeDisposable _Disposables;
    readonly UniRx.CompositeDisposable _BattleDisposables;

    public TestBattle()
    {
        _Disposables = new CompositeDisposable();
        _BattleDisposables = new CompositeDisposable();
    }

    // Start is called before the first frame update
    void Start()
    {
        NotifierRx.ToObservable().Supply<IBattle>().Subscribe(_ShowBattle).AddTo(_Disposables);
        NotifierRx.ToObservable().Unsupply<IBattle>().Subscribe(_HideBattle).AddTo(_Disposables);
        Battle.OnClickAsObservable().Subscribe(_ => _GetRsult()).AddTo(_BattleDisposables);
    }

    private void OnDestroy()
    {
        _Disposables.Clear();
        _BattleDisposables.Clear();
    }
    private void _HideBattle(IBattle obj)
    {
        Root.SetActive(false);
    }

    private void _ShowBattle(IBattle obj)
    {
        Root.SetActive(true);
        _BattleDisposables.Clear();
    }

    private void _GetRsult()
    {
        var r = from notifier in NotifierRx.ToObservable()
                from battle in notifier.QueryNotifier<IBattle>().SupplyEvent()
                from result in battle.RequestBattleResult().RemoteValue()
                select result;

        r.DefaultIfEmpty(new BattleResult()).Subscribe(_PrintResult).AddTo(_BattleDisposables);
    }

    private void _PrintResult(BattleResult result)
    {
        foreach (var act in result.Actions)
        {
            Debug.Log(act.ToString());
        }
    }
}
