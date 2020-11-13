using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using System;

namespace Phoenix.Project1.Battles
{
    internal class Battle : IBattle
    {
        readonly Regulus.Utility.StatusMachine _Machine;
        bool _Battleing;
        readonly Battles.Stage _Stage;
        private readonly IBinder _Binder;
        readonly BattleTime _Time;

        
        public Battle(Stage stage,IBinder binder)
        {
            _Stage = stage;
            this._Binder = binder;
            _Time = new BattleTime();
            _Machine = new StatusMachine();
            _StageProperty = new Property<int>(_Stage.Id);
            
            _Entrances = new NotifierCollection<ActorEntranceTimestamp>();            
            _ActorPerforms = new NotifierCollection<ActorPerformTimestamp>();
        
            _Finishs = new NotifierCollection<BattleResult>();
            _Actors = new NotifierCollection<IActor>();
            
            
        }

        

        public void Step()
        {
            _Machine.Update();
        }
        public void Start()
        {
            _Actors.Items.Clear();

            _ActorPerforms.Items.Clear();
        
            _Finishs.Items.Clear();            
            _Entrances.Items.Clear();

            _Time.Reset();
            _Battleing = true;

            foreach (var actor in _Stage.GetActors())
            {
                _Actors.Items.Add(actor);
            }
            _ActorCount.Value = _Actors.Items.Count;
            _ToReady();
        }

        private void _ToReady()
        {
            var status = new BattleReady(_Binder);
            status.DoneEvent += _ToEntrance;
            _Machine.Push(status);
        }

        public void End()
        {
            while(_Battleing)
            {
                _Machine.Update();
            }
            
            _Machine.Termination();
        }

        private void _ToEntrance()
        {
            var status = new BattleEntrance(_Time, _Stage);
            status.DoneEvent += (f)=>_ToReferee();
            status.EntranceEvent += a => _Entrances.Items.Add(a);
            _Machine.Push(status);
        }

        void _ToReferee()
        {
            var status = new BattleReferee(_Stage);
            status.VictoryEvent += _ToFinish;
            status.PerformEvent += _ToPerform;
            _Machine.Push(status);
        }

        private void _ToPerform(Actor actor)
        {            
            var status = new BattlePerform(actor, _Stage,_Time);
            status.PerformEvent += a => _ActorPerforms.Items.Add(a);
            status.DoneEvent += _ToReferee;
            _Machine.Push(status);
        }

        void _ToFinish(Winner winner)
        {
            _Battleing = false;
            _Finishs.Items.Add(new BattleResult { Winner = winner });
            _Machine.Empty();
        }

        void IBattle.Pass()
        {
            if(_Battleing)
                End();
        }

        readonly Regulus.Remote.Property<int> _StageProperty;
        Regulus.Remote.Property<int> IBattle.Stage => _StageProperty;

        readonly Phoenix.Project1.NotifierCollection<IActor> _Actors;
        INotifier<IActor> IBattle.Actors => _Actors;

        
        Regulus.Remote.Property<int> IBattle.Frames => _Time.Frames;

        readonly Property<int> _ActorCount ;
        Property<int> IBattle.ActorCount => _ActorCount;

        readonly Phoenix.Project1.NotifierCollection<ActorEntranceTimestamp> _Entrances;

        event Action<ActorEntranceTimestamp> IBattle.EntranceEvent
        {
            add
            {
                _Entrances.Notifier.Supply += value;
            }

            remove
            {
                _Entrances.Notifier.Supply -= value;
            }
        }

        readonly Phoenix.Project1.NotifierCollection<ActorPerformTimestamp> _ActorPerforms;
        event Action<ActorPerformTimestamp> IBattle.ActorPerformEvent
        {
            add
            {
                _ActorPerforms.Notifier.Supply += value;
            }

            remove
            {
                _ActorPerforms.Notifier.Supply -= value;
            }
        }

        
        readonly Phoenix.Project1.NotifierCollection<BattleResult> _Finishs;
        event Action<BattleResult> IBattle.FinishEvent
        {
            add
            {
                _Finishs.Notifier.Supply += value;
            }

            remove
            {
                _Finishs.Notifier.Supply -= value;
            }
        }

        
      
    }
}
