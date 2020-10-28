namespace Phoenix.Project1.Client.Battles
{    
    public class StateTransition
    {
        private BattleStateBase _DestinationState;

        private BattleStateMachine _DestinationStateMachine;
        
        public void SetDestinationState(BattleStateBase state)
        {
            _DestinationState = state;
        }

        public void SetDestinationStateMachine(BattleStateMachine stateMachine)
        {
            _DestinationStateMachine = stateMachine;
        }
                
        public BattleStateMachine GetDestinationStateMachine()
        {
            return _DestinationStateMachine;
        }

        public BattleStateBase GetDestinationState()
        {
            return _DestinationState;
        }
    }
}