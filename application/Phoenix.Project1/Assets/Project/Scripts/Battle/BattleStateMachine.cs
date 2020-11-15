using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleStateMachine : IDisposable
    {
        public enum PlayMode
        {
            Sequence,
            Parallel
        }
        
        public PlayMode MachinePlayMode = PlayMode.Sequence;
        
        private List<BattleStateBase> _States;

        private BattleStateBase _CurrentState;

        public event Action<BattleStateMachine> FinishedCallaback;

        public CompositeDisposable Disposable;
        
        public BattleStateMachine()
        {
            _States = new List<BattleStateBase>();
            Disposable = new CompositeDisposable();
        }

        public void Update()
        {
            _CurrentState?.Update();
        }       

        public BattleStateBase AddState(BattleStateBase state)
        {
            if (_States.Exists(x => x.ID == state.ID))
            {
                Debug.LogError("Can not add same state in to stateMachine");
                return state;
            }           

            _States.Add(state);
            
            if (_States.Count == 1)
            {
                _CurrentState = state;
            }

            return state;
        }

        public void Start()
        {
            Play(_CurrentState);            
        }

        public void RemoveState(BattleStateBase state)
        {
            _States.Remove(state);
        }

        public bool HasState(BattleStateBase battleStateBase)
        {
            return _States.Exists(x => x.ID == battleStateBase.ID);
        }

        public void Play(BattleStateBase stateBase)
        {           
            _CurrentState = stateBase;
            
            if (_CurrentState == null)
            {
                FinishedCallaback?.Invoke(this);
                return;
            }

            _CurrentState.Start();
        }

        public void Dispose()
        {
            _CurrentState?.Dispose();

            foreach (var state in _States)
            {
                state.Dispose();
            }
            Disposable?.Dispose();
        }
    }

    public static class BattleStateMachineRx
    {
        public static IObservable<BattleStateMachine> FinishedAsObservable(this BattleStateMachine stateMachine)
        {
            return UniRx.Observable.FromEvent<Action<BattleStateMachine>, BattleStateMachine>(h => (current) => h(current), h => stateMachine.FinishedCallaback += h, h => stateMachine.FinishedCallaback -= h);            
        }
    }

}
