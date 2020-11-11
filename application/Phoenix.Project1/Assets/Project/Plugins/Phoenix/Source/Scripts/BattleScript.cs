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
            var obs = from dashboard in _Queryable.QueryNotifier<IDashboard>().SupplyEvent()
                      from _ in _RequestBattle(dashboard)
                      from battle in _Queryable.QueryNotifier<IBattle>().SupplyEvent()
                      select battle;
            obs.Subscribe(_Battle).AddTo(_Disposables);
        }

        private void _Battle(IBattle battle)
        {
            var actorPerformObs = UniRx.Observable.FromEvent<Action<ActorPerformTimestamp> , ActorPerformTimestamp>(h => (gpi) => h(gpi), h => battle.ActorPerformEvent += h, h => battle.ActorPerformEvent -= h);
            var enterenceObs =  UniRx.Observable.FromEvent<Action<ActorEntranceTimestamp>, ActorEntranceTimestamp>(h => (gpi) => h(gpi), h => battle.EntranceEvent += h, h => battle.EntranceEvent -= h);
            var finishObs = UniRx.Observable.FromEvent<Action<BattleResult>, BattleResult>(h => (gpi) => h(gpi), h => battle.FinishEvent += h, h => battle.FinishEvent -= h);

            actorPerformObs.Subscribe(_Print).AddTo(_Disposables);
            enterenceObs.Subscribe(_Print).AddTo(_Disposables);
            finishObs.Subscribe(_Print).AddTo(_Disposables);

        }

        private void _Print(BattleResult obj)
        {
            ;
            _Viewer.WriteLine($"BattleResult : {Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented)}");            
        }

        private void _Print(ActorEntranceTimestamp obj)
        {
            _Viewer.WriteLine($"ActorEntranceTimestamp : {Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented)}");
        }

        private void _Print(ActorPerformTimestamp obj)
        {
            //_Viewer.WriteLine($"ActorPerformTimestamp : Frame:{obj.Frames} , StarringId:{obj.ActorPerform.StarringId} SpellId:{obj.ActorPerform.SpellId} EffectTargetId:{obj.ActorPerform.TargetEffects[0].Effects[0].Actor}");

            _Viewer.WriteLine($"ActorPerformTimestamp : {Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented)}");
        }
    }
}