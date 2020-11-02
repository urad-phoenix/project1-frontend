using System;
using System.Collections.Generic;

namespace Phoenix.Project1.Client.Battles
{
    public abstract class BattleStateBase
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
        
        public BattleStateBase(string name)
        {
            _Name = name;
            
            _Id = Guid.NewGuid();                      
        }

        public void AddTransition(StateTransition transition)
        {
            _Transition = transition;
        }
        
        public void RemoveTransition()
        {
            _Transition = null;
        }

        public StateTransition CreateTransition(BattleStateBase destinationState)
        {
            var newTransition = new StateTransition();

            return newTransition;
        }
        
        public abstract void Start();

        public abstract void Stop();

        public abstract void Update();                
    }
}