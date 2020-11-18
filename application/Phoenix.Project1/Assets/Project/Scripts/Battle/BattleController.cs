using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Phoenix.Playables;
using Phoenix.Pool;
using Phoenix.Project1.Addressable;
using Phoenix.Project1.Common.Battles;
using Project.Scripts.UI;
using Regulus.Remote.Reactive;
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
        private CompositeDisposable _Disposables;
        
        private readonly UniRx.CompositeDisposable _SendDisposables;
        
        private readonly UniRx.CompositeDisposable _SendRequestDisposables; 
          
        private string _PlayablePoolName = "BattlePlayablePool";

        private int _PlayablePoolSize = 10;

        private ObjectPool _DirectorPool;

        private BattleStateMachine _CurrentStateMachine;

        private List<Avatar> _Avatars;

        private Queue<BattleStateMachine> _Machines;
        
        [SerializeField]
        private CharacterLocator[] _Locators;

        [SerializeField] 
        private Camera _Camera;

        [SerializeField] 
        private CameraGroup _StageCameraGroup;
        
        public class LoadData
        {
            public int Id;

            public int Location;

            public AsyncOperationHandle<GameObject> Handle;
        }
        
        public BattleController()
        {                           
            _Machines = new Queue<BattleStateMachine>();
            
            _Disposables = new CompositeDisposable();
            
            _SendDisposables = new CompositeDisposable();
            
            _SendRequestDisposables = new CompositeDisposable();
            
            _Avatars = new List<Avatar>();
        }

        void Start()
        {
            ActorUIController.Instance.SettingCamera(_Camera);
            
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
            _DirectorPool.Initialize();
            _DirectorPool.Spawn();
        }     

        private IObservable<LoadData[]> _SpawnRole(IList<IActor> actors)
        {            
            List<IObservable<LoadData>> loaders = new List<IObservable<LoadData>>();
            
            foreach (var actor in actors)
            {
                var locator = _Locators.First(x => x.Index == actor.Location);

                locator.SetInstanceID(actor.InstanceId);

                var role = locator.GetRole();            
            
                var avatarId = actor.AvatarId.Value;

                var loadData = from hnd in UniRx.Observable.Defer(() =>
                        Addressables.InstantiateAsync(avatarId, role.transform).AsHandleObserver())
                        select new LoadData() {Id = actor.InstanceId, Location = actor.Location, Handle = hnd};
                               
                loaders.Add(loadData);
            }
                      
            var obs = from handle in UniRx.Observable.Merge(loaders)
                from observable in _RoleLoaded(handle)
                where observable
                select handle;

            return Observable.WhenAll(obs);                            
        }

        private IObservable<bool> _RoleLoaded(LoadData data)
        {    
            if (!data.Handle.IsDone)
            {
                return Observable.Return(false);
            }           
            
            Debug.Log($"_RoleLoaded {data.Id}");
            
            data.Handle.Result.SetActive(false);
            
            var avatar = data.Handle.Result.GetComponent<Avatar>();

            avatar.InstanceID = data.Id;
            avatar.Location = data.Location;
            _Avatars.Add(avatar);
            
            ActorUIController.Instance.SettingHUD(avatar);
            
            return Observable.Return(true);
        }

        private void _Battle(IBattle battle)
        {            
            var actorPerformObs = UniRx.Observable.FromEvent<Action<ActorPerformTimestamp> , ActorPerformTimestamp>(h => (gpi) => h(gpi), h => battle.ActorPerformEvent += h, h => battle.ActorPerformEvent -= h);
            var enterenceObs =  UniRx.Observable.FromEvent<Action<ActorEntranceTimestamp>, ActorEntranceTimestamp>(h => (gpi) => h(gpi), h => battle.EntranceEvent += h, h => battle.EntranceEvent -= h);
            var finishObs = UniRx.Observable.FromEvent<Action<BattleResult>, BattleResult>(h => (gpi) => h(gpi), h => battle.FinishEvent += h, h => battle.FinishEvent -= h);

            var actors = from actor in battle.Actors.SupplyEvent()                            
                         select actor;
            
            var actorHpObs = from actor in actors
                from newHp in actor.Hp.ObserveEveryValueChanged(h=>h.Value)
                select new { actor, newHp };

            actorHpObs.DoOnError(_Error)
                .Subscribe(v => _ChangeActorHp(v.actor.InstanceId.Value, v.newHp)); //.AddTo(_Disposables);

            var loadObs = from load in actors.Buffer(battle.ActorCount.Value)
                from spawn in _SpawnRole(load)
                select spawn;
            
            loadObs.DoOnError(_Error).Subscribe(_Ready).AddTo(_Disposables);
                        
            actorPerformObs.DoOnError(_Error).Subscribe(_ActorPerform).AddTo(_Disposables);
            
            enterenceObs.DoOnError(_Error).Subscribe(_BattleEntrance).AddTo(_Disposables);
            
            finishObs.DoOnError(_Error).Subscribe(_BattleFinished).AddTo(_Disposables);
            
           
        }      

        private void _Ready(LoadData[] handles)
        {
            Debug.Log("Ready");           

            var readyObs = from ready in NotifierRx.ToObservable().Supply<IReady>()
                            select ready;
            
            readyObs.Subscribe(r => r.Ready()).AddTo(_Disposables);
        }
     
        private void _ChangeActorHp(int id, int newHp)
        {           
            Debug.LogError($"Actor {id} Hp = {newHp}");
            
            ActorUIController.Instance.SetCurrentBlood(id, newHp);
        }
        
        private void _Error(Exception obj)
        {
            Debug.LogError(obj.Message);
        }
        
        private void _BattleFinished(BattleResult obj)
        {            
            Debug.Log($"_BattleFinished : {obj.Winner}");
            
            var stateMachine = new BattleStateMachine();
            
            stateMachine.AddState(new BattleResultState("finished", stateMachine));          
            
            Enqueue(stateMachine, true);
        }       

        private void _BattleEntrance(ActorEntranceTimestamp obj)
        {                                    
            
            foreach (var entrance in obj.ActorEntrances)
            {                
                var avatar = GetAvatarByID(entrance.Id);
                
                avatar.gameObject.SetActive(true);                           
            }
            
            Debug.Log($"_BattleEntrance : {obj.ActorEntrances.Count()}");          
        }

        private void _ActorPerform(ActorPerformTimestamp obj)
        {                     
            Debug.Log($"_ActorPerform location : {obj.ActorPerform.Location}");

            var stateMachine = new BattleStateMachine();
            stateMachine.
            AddState(new MoveState($"move{obj.Frames.ToString()}", stateMachine, new MoveData()
            {
                MoveActorId = obj.ActorPerform.StarringId,
                Location = obj.ActorPerform.Location
            }, this)).
            AddNext(stateMachine.AddState(new BattleActState($"act{obj.Frames.ToString()}", stateMachine, new ActData()
            {
                ActKey = (ActionKey) obj.ActorPerform.SpellId,
                Location = obj.ActorPerform.Location,
                ActorId = obj.ActorPerform.StarringId
            }, this))).
            AddNext(stateMachine.AddState(new BackMoveState($"move{obj.Frames.ToString()}", stateMachine, new MoveData()
                {
                    MoveActorId = obj.ActorPerform.StarringId,
                    Location = GetLocatorIndex(obj.ActorPerform.StarringId)
                }, this)));                      
            
            Enqueue(stateMachine);
        }
               
        private void _StateMachineFinished(BattleStateMachine stateMachine)
        {           
            stateMachine.Dispose();
            stateMachine = null;

            _CurrentStateMachine = null;
            
            if (_Machines.Count != 0)
            {                
                _CurrentStateMachine = _Machines.Dequeue();
                _CurrentStateMachine.Start();
            }
        }

        private void Enqueue(BattleStateMachine stateMachine, bool isLast = false)
        {                        
            if(stateMachine == null)
                return;
            
            var finishedObs = stateMachine.FinishedAsObservable();

            if (isLast)
            {
                finishedObs.Subscribe(_EndBattle);
            }
            else
            {
                finishedObs.Subscribe(_StateMachineFinished);    
            }

            if (_Machines.Count == 0 && _CurrentStateMachine == null)
            {
                _CurrentStateMachine = stateMachine;
                
                stateMachine.Start();
            }
            else
            {
                _Machines.Enqueue(stateMachine);    
            }
        }

        private void Update()
        {
            _CurrentStateMachine?.Update();
        }      

        public int GetLocatorIndex(int id)
        {
            return _Locators.First(x => x.GetInstanceID() == id).Index;
        }

        public Role GetRole(int id)
        {
            return _Locators.First(x => x.GetInstanceID() == id).GetRole();
        }

        public CharacterLocator GetLocator(int index)
        {
            return _Locators.First(x => x.Index == index);
        }

        public Avatar GetAvatarByID(int id)
        {
            return _Avatars.Find(x => x.InstanceID == id);
        } 
        
        public Avatar GetAvatarByLocation(int index)
        {
            return _Avatars.Find(x => x.Location == index);
        }  

        public Camera GetMainCamera()
        {
            return _Camera;
        }

        public CinemachineVirtualCamera GetVirtualCamera(bool isActorCamera, int index)
        {
            if (isActorCamera)
            {
                var avatar = GetAvatarByLocation(index);

                return avatar.GetVirtualCamera(index);
            }
            else
            {
                return _StageCameraGroup.GetVirtualCamera(index);
            }           
        }

        public PlayableDirector GetPlayableDirector(ActionKey actionKey, int id)
        {            
            var avatar = GetAvatarByID(id);

            try
            {
                var timelineData = avatar.TimelineAssets.First(x => x.Action == actionKey);
            
                var go = _DirectorPool.Get(true);
            
                var director = go.GetComponent<PlayableDirector>();

                director.playableAsset = timelineData.TimelineAsset;
            
                return director;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }            
        }

        public void PlaySound(string key)
        {
        }                

        public void RecyclePlayableDirector(PlayableDirector playableDirector)
        {                       
            _DirectorPool.Recycle(playableDirector.gameObject, false);
        }

        private void _Finished()
        {
            _Disposables.Clear();
            
            _SendDisposables.Clear();
            
            _SendRequestDisposables.Clear();
            
            _Machines.Clear();            
        }

        private void OnDestroy()
        {
            
            _Finished();
        }
        
        private void _EndBattle(BattleStateMachine stateMachine)
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattleStatus>()
                select battle;
            
            battleObs.Subscribe(_ExitBattle).AddTo(_Disposables);                        
        }

        private void _ExitBattle(IBattleStatus battle)
        {
            foreach (var avatar in _Avatars)
            {
                Addressables.ReleaseInstance(avatar.gameObject);
            }

            foreach (var machine in _Machines)
            {
                machine.Dispose();
            }
            
            battle.Exit();      
        }      
    }
}