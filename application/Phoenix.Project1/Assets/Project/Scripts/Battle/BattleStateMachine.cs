using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleStateMachine
    {        
        private List<BattleStateBase> _States = new List<BattleStateBase>();

        private BattleStateBase _CurrentState;

        public List<BattleStateMachine> _SubStateMachines;
        
        public BattleStateMachine()
        {
            _States = new List<BattleStateBase>();
            _SubStateMachines = new List<BattleStateMachine>();
        }

        public void Update()
        {
            foreach (var state in _States)
            {
                state.Update();
            }
        }

        public void AddSubStateMachine(BattleStateMachine machine)
        {
            _SubStateMachines.Add(machine);
        }

        public void AddState(BattleStateBase state)
        {
            if (_States.Exists(x => x.ID == state.ID))
            {
                Debug.LogError("Can not add same state in to stateMachine");
                return;
            }           

            _States.Add(state);
            
            if (_States.Count == 1)
            {
                _CurrentState = state;
            }
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
            if(_CurrentState != null)
                _CurrentState.Stop();
            
            stateBase.Start();
            _CurrentState = stateBase;
        }
    }
}
