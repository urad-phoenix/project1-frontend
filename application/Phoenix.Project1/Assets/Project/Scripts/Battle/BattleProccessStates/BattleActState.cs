using Phoenix.Project1.Client.Battles;
using Phoenix.Project1.Common.Battles;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleActState : BattleStateBase
    {
        private Action _Action;        
        
        public BattleActState(string name, Action action) : base(name)
        {
            _Action = action;
            
        }

        public void AddBehaviour(IStateBehaviour behaviour)
        {
            
        }

        public override void Start()
        {
            if (_Action == null)
            {
                _Finished();
                return;
            }
            
                        
        }

        private void _Finished()
        {
            var destination = _Transition.GetDestinationState();
            
            var stateMachine = _Transition.GetDestinationStateMachine();
            
            stateMachine.Play(destination);
        }

        public override void Stop()
        {           
        }

        public override void Update()
        {            
        }
    }
}