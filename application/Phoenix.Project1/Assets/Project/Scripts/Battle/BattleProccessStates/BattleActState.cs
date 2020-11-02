using Phoenix.Project1.Client.Battles;
using Phoenix.Project1.Common.Battles;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleActState : BattleStateBase
    {
        private Action _Action;

        private IStateBehaviour _Behaviour;

        private StateBinding _StateBinding;
        
        public BattleActState(string name, Action action, StateBinding binding) : base(name)
        {
            _Action = action;
            
            _StateBinding = binding;

            var controller = binding.GetHandle().GetReferenceObject() as BattleController;

            //var director = controller.GetPlayableDirector(_Action.SkillId);
            
            //director.

        }

        public override void Start()
        {
            if (_Action == null)
            {
                _Finished();
                return;
            }
            
            _Behaviour.Start(_StateBinding);                        
        }

        public override void Stop()
        {
            _Behaviour.Stop(_StateBinding);
        }

        public override void Update()
        {
            _Behaviour.Update(_StateBinding);
        }
        
        private void _Finished()
        {
            var destination = _Transition.GetDestinationState();
            
            var stateMachine = _Transition.GetDestinationStateMachine();
            
            stateMachine.Play(destination);
        }
    }
}