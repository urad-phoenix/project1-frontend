using Phoenix.Project1.Common.Battles;
using Regulus.Utility;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class BattleController : MonoBehaviour
    {
        private BattleStateMachine _StateMachine;

        private CompositeDisposable _disposable;
        
        public BattleController()
        {
            _StateMachine = new BattleStateMachine();
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattle>()
                select battle;

            battleObs.Subscribe(_GetBattle).AddTo(_disposable);
        }

        private void _GetBattle(IBattle battle)
        {
            var result = battle.RequestBattleResult();

            var battleResult = result.GetValue();

            if (battleResult != null && battleResult.Status == BattleStatus.Success)
            {
                var teams = battleResult.Teams;

                //TODO
                //產生角色對應到站位

                
                var actions = battleResult.Actions;

                for (int i = 0; i < actions.Length; ++i)
                {
                    var action = actions[i];                    
                    
                    //_StateMachine.AddState(new BattleActState());    
                }
                
                
            }
        }

        private void Start()
        {
            //_StateMachine.AddState();
        }

        public void GetUnit(string id)
        {
            
        }

        public void GetCamera(string id)
        {
            
        }

        private void _Finished()
        {
            _disposable.Clear();
        }

        private void OnDestroy()
        {
            _Finished();
        }
    }
}