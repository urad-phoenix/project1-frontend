using Phoenix.Project1.Client.Battles;
using Phoenix.Project1.Common.Battles;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleActState : BattleStateBase
    {
        private Action[] _Actions;

        private BehaviourHandle _BehaviourHandle;
        
        public BattleActState(string name, BehaviourHandle behaviourHandle, Action[] actions) : base(name)
        {
            _Actions = actions;
            _BehaviourHandle = behaviourHandle;
        }

        public override void Start()
        {
            if (_Actions == null)
            {
                _Finished();
                return;
            }

            var controller = _BehaviourHandle.GetReferenceObject() as BattleController;
            
            for (int i = 0; i < _Actions.Length; ++i)
            {
                var action = _Actions[i];
                
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