using System;
using System.Linq;
using Phoenix.Pool;
using Phoenix.Project1.Addressable;
using Phoenix.Project1.Client.UI;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote.Reactive;
using Regulus.Utility;
using TP.Scene.Locators;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleController : MonoBehaviour
    {
        private BattleStateMachine _StateMachine;

        private CompositeDisposable _Disposables;
        
        private readonly UniRx.CompositeDisposable _SendDisposables;
        
        private readonly UniRx.CompositeDisposable _SendRequestDisposables; 
        
        private readonly Regulus.Utility.Console.IViewer _Viewer;

        private string _PlayablePoolName = "BattlePlayablePool";

        private int _PlayablePoolSize = 10;

        private ObjectPool _DirectorPool;

        [SerializeField]
        private CharacterLocator[] _Locators;                
        
        public BattleController()
        {
            _StateMachine = new BattleStateMachine();                        
            
            _Disposables = new CompositeDisposable();
            
            _SendDisposables = new CompositeDisposable();
            
            _SendRequestDisposables = new CompositeDisposable();
        }

        void Start()
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattle>()                
                            select battle;   
                        

            battleObs.Subscribe(_Battle).AddTo(_Disposables);

            _SetPlayablePool();
        }

        private void _SetPlayablePool()
        {
            var playable = new GameObject("BattlePlayObject");

            var director = playable.AddComponent<PlayableDirector>();
            
            director.playOnAwake = false;
            
            _DirectorPool = new ObjectPool(_PlayablePoolName, playable, this.transform, _PlayablePoolSize, true);
            
            _DirectorPool.Spawn();
        }

        private void _SpawnRole(IActor actor)
        {
            var locator = _Locators.First(x => x.Index == actor.Location);

            var role = locator.GetRole();

            role.ID = actor.InstanceId;
            
            var loader = Addressables.InstantiateAsync("hero-1", role.transform).AsHandleObserver();

            loader.Subscribe(RoleLoaded).AddTo(_Disposables);
        }

        private void RoleLoaded(AsyncOperationHandle<GameObject> handle)
        {
            if(handle.IsDone && handle.Result == null)
                return;
                        
            handle.Result.SetActive(false);                       
        }

        private void _Battle(IBattle battle)
        {
            var actorPerformObs = UniRx.Observable.FromEvent<Action<ActorPerformTimestamp> , ActorPerformTimestamp>(h => (gpi) => h(gpi), h => battle.ActorPerformEvent += h, h => battle.ActorPerformEvent -= h);
            var enterenceObs =  UniRx.Observable.FromEvent<Action<ActorEntranceTimestamp>, ActorEntranceTimestamp>(h => (gpi) => h(gpi), h => battle.EntranceEvent += h, h => battle.EntranceEvent -= h);
            var finishObs = UniRx.Observable.FromEvent<Action<BattleResult>, BattleResult>(h => (gpi) => h(gpi), h => battle.FinishEvent += h, h => battle.FinishEvent -= h);

            var actors = from actor in battle.Actors.SupplyEvent()
                select actor;

            actors.Subscribe(_SpawnRole).AddTo(_Disposables);
            
            
            actorPerformObs.DoOnError(_Error).Subscribe(_ActorPerform).AddTo(_Disposables);
            enterenceObs.DoOnError(_Error).Subscribe(_BattleEntrance).AddTo(_Disposables);
            finishObs.DoOnError(_Error).Subscribe(_BattleFinished).AddTo(_Disposables);
            
            var actorHpObs = from actor in battle.Actors.SupplyEvent()
                from newHp in actor.Hp.ChangeObservable()
                select new { actor, newHp };
            actorHpObs.Subscribe(v => _ChangeActorHp(v.actor.InstanceId.Value , v.newHp)).AddTo(_Disposables);
        }
        
        private void _ChangeActorHp(int id, int newHp)
        {
            _Viewer.WriteLine($"Actor {id} Hp = {newHp}");
        }
        
        private void _Error(Exception obj)
        {
            _Viewer.WriteLine(obj.Message);
        }
        
        private void _BattleFinished(BattleResult obj)
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

        private void _BattleEntrance(ActorEntranceTimestamp obj)
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            _Viewer.WriteLine($"ActorEntranceTimestamp :");
            foreach (var item in message.Split('\n'))
            {
                _Viewer.WriteLine(item);
            }
            _Viewer.WriteLine("");
        }

        private void _ActorPerform(ActorPerformTimestamp obj)
        {
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            _Viewer.WriteLine($"ActorPerformTimestamp :");
            foreach (var item in message.Split('\n'))
            {
                _Viewer.WriteLine(item);
            }
            _Viewer.WriteLine("");
        }
        
        private void _EndBattle()
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattleStatus>()
                select battle;
            
            battleObs.Subscribe(_ExitBattle).AddTo(_Disposables); 
        }

        private void _ExitBattle(IBattleStatus battle)
        {
            battle.Exit();
            //battle.Exit();
        } 

        private void _GetBattle(IBattle battle)
        {            
            /*
            //TODO
            //關卡資訊, 之後要移到關卡腳本
            var stage = battle.GetCampsByStageId(1);
            
            var stageResult = stage.GetValue();

            if (stageResult == null)
            {                
                _EndBattle();
                return;
            }
            //關卡初始化, 產生角色, 定位
            
            //拿整場戰鬥的結果
            var fight = battle.ToFight(1);            
                
            var battleResult = fight.GetValue();

            //BattleStatus.Success贏  BattleStatus.Fail 輸
            if (battleResult != null)
            {                
                var actions = battleResult.Actions;                

                for (int i = 0; i < actions.Length; ++i)
                {
                    var action = actions[i];
                    
                    var handle = new BindingHandle();
                    
                    handle.SetReferenceObject(this);

                    var binding = new StateBinding(handle);
                    
                    var state = new BattleActState(action.SkillId.ToString(), action, binding);
                   
                    _StateMachine.AddState(state);
                }

                _SetBattleFinished();
            }                       */ 
        }

        void _SetBattleFinished()
        {
            //TODO
            //跟server要獎勵或是第一次同步就已經同步獎勵完成
            _StateMachine.AddState(new BattleResultState("finished"));
        }              

        public Role GetRole(string id)
        {
            return null;
        }

        public CameraUnit GetCamera(string id)
        {
            return null;
        }

        public PlayableDirector GetPlayableDirector(string skillId)
        {
            return null;
        }

        public void PlaySound(string key)
        {
        }                

        public void RecyclePlayableDirector(PlayableDirector playableDirector)
        {
            //playableDirector.re
        }

        private void _Finished()
        {
            _Disposables.Clear();
            
            _SendDisposables.Clear();
            
            _SendRequestDisposables.Clear();
        }

        private void OnDestroy()
        {
            _Finished();
        }
    }
}