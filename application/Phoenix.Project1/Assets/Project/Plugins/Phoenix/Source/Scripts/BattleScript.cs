using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Battles;
using Regulus.Extension;
using Regulus.Remote;
using Regulus.Remote.Reactive;
using System;
using UniRx;

namespace Phoenix.Project1.Client.Scripts
{
    internal class BattleScript : IScriptable
    {
        private readonly INotifierQueryable _Queryable;
        private readonly Regulus.Utility.Console.IViewer _Viewer;
        readonly UniRx.CompositeDisposable _Disposables;

        public BattleScript(INotifierQueryable queryable, Regulus.Utility.Console.IViewer viewer)
        {
            _Disposables = new CompositeDisposable();
            this._Queryable = queryable;
            this._Viewer = viewer;
        }
        void IScriptable.End()
        {
            _Disposables.Clear();
        }
        IObservable<UniRx.Unit> _RequestBattle(IDashboard dashboard)
        {
            dashboard.RequestBattle();
            return UniRx.Observable.Return(UniRx.Unit.Default);
        }
        void IScriptable.Start()
        {
            _Disposables.Clear();
            var obs = from dashboard in _Queryable.QueryNotifier<IDashboard>().SupplyEvent()
                      from _ in _RequestBattle(dashboard)
                      from battle in _Queryable.QueryNotifier<IBattle>().SupplyEvent()
                      from ready in _Queryable.QueryNotifier<IReady>().SupplyEvent()
                      select new { battle, ready };
            obs.DoOnError(_Error).Subscribe(v=>_Battle(v.battle , v.ready)).AddTo(_Disposables);
        }

        private void _Error(Exception obj)
        {
            _Viewer.WriteLine(obj.Message);
        }

        private void _Battle(IBattle battle,IReady ready)
        {
            ready.Ready();
            var actorPerformObs = UniRx.Observable.FromEvent<Action<ActorPerformTimestamp> , ActorPerformTimestamp>(h => (gpi) => h(gpi), h => battle.ActorPerformEvent += h, h => battle.ActorPerformEvent -= h);
            var enterenceObs =  UniRx.Observable.FromEvent<Action<ActorEntranceTimestamp>, ActorEntranceTimestamp>(h => (gpi) => h(gpi), h => battle.EntranceEvent += h, h => battle.EntranceEvent -= h);
            var finishObs = UniRx.Observable.FromEvent<Action<BattleResult>, BattleResult>(h => (gpi) => h(gpi), h => battle.FinishEvent += h, h => battle.FinishEvent -= h);

            actorPerformObs.DoOnError(_Error).Subscribe(_Print).AddTo(_Disposables);
            enterenceObs.DoOnError(_Error).Subscribe(_Print).AddTo(_Disposables);
            finishObs.DoOnError(_Error).Subscribe(_Print).AddTo(_Disposables);
            var actorHpObs = from actor in battle.Actors.SupplyEvent()
                            from newHp in actor.Hp.ChangeObservable()
                            select new { actor, newHp };
            actorHpObs.Subscribe(v => _PrintActorHp(v.actor.InstanceId.Value , v.newHp)).AddTo(_Disposables);
        }

        private void _PrintActorHp(int id, int newHp)
        {
            _Viewer.WriteLine($"Actor {id} Hp = {newHp}");
        }

        private void _Print(BattleResult obj)
        {
            
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            _Viewer.WriteLine($"BattleResult :");
            foreach (var item in message.Split('\n'))
            {
                _Viewer.WriteLine(item);
            }            
            _Viewer.WriteLine("");

            _Disposables.Clear();
        }

        private void _Print(ActorEntranceTimestamp obj)
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            _Viewer.WriteLine($"ActorEntranceTimestamp :");
            foreach (var item in message.Split('\n'))
            {
                _Viewer.WriteLine(item);
            }
            _Viewer.WriteLine("");
        }

        private void _Print(ActorPerformTimestamp obj)
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            _Viewer.WriteLine($"ActorPerformTimestamp :");
            foreach (var item in message.Split('\n'))
            {
                _Viewer.WriteLine(item);
            }
            _Viewer.WriteLine("");
        }
    }
}