using Phoenix.Project1.Client.Battles;
using Phoenix.Project1.Common.Battles;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleActState : BattleStateBase
    {
        private ActData _ActData;

        private IStateBehaviour _Behaviour;

        private StateBinding _StateBinding;
        
        public BattleActState(string name, ActData actData, StateBinding binding) : base(name)
        {
            _ActData = actData;
            
            _StateBinding = binding;

            var controller = binding.GetHandle().GetReferenceObject() as BattleController;

            var director = controller.GetPlayableDirector(_ActData.ActKey, _ActData.ActorId);
            
            
            //director.

        }

        public override void Start()
        {
            if (_ActData == null)
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