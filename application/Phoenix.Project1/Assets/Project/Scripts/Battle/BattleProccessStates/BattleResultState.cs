using UniRx;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleResultState : BattleStateBase
    {     
        private CompositeDisposable _Disposable;
        
        public BattleResultState(string name, BattleStateMachine stateMachine) : base(name, stateMachine)
        {
            _Disposable = new CompositeDisposable();
        }

        public override void Start()
        {
            _SwitchState();
        }
        
        public override void Update()
        {           
        }

        public override void Dispose()
        {
            _Disposable.Clear();
        }
    }
}