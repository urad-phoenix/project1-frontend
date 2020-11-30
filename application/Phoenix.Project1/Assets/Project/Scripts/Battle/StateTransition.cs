namespace Phoenix.Project1.Client.Battles
{    
    public class StateTransition
    {
        private BattleStateBase _DestinationState;

        private BattleStateMachine _DestinationStateMachine;

        public StateTransition(BattleStateBase state, BattleStateMachine stateMachine)
        {
            _DestinationState = state;

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