using System;
using System.Collections.Generic;

namespace Phoenix.Project1.Client.Battles
{
    public abstract class BattleStateBase : IDisposable
    {
        protected StateTransition _Transition;               
        
        public Guid ID
        {
            get { return _Id; }
        }

        public string Name
        {
            get { return _Name; }            
        }

        protected readonly System.Guid _Id;

        private string _Name;      

        protected BattleStateMachine _StateMachine;
        
        public BattleStateBase(string name,  BattleStateMachine stateMachine)
        {
            _Name = name;
            _StateMachine = stateMachine;
            _Id = Guid.NewGuid();  
        }

        public BattleStateBase AddNext(BattleStateBase destinationState)
        {
            _Transition = new StateTransition(destinationState, _StateMachine);

            return _Transition.GetDestinationState();
        }                
        
        public void RemoveTransition()
        {
            _Transition = null;
        }      
        
        public abstract void Start();

        public virtual void Stop()
        {
        }

        protected virtual void _SwitchState()
        {  
            var destination = _Transition?.GetDestinationState();
                                    
            _StateMachine.Play(destination);
        }

        public abstract void Update();

        public abstract void Dispose();
    }
}