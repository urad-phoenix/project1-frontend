using Phoenix.Project1.Common.Battles;
using Regulus.Utility;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

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
            //TODO
            //關卡資訊, 之後要移到關卡腳本
            var stage = battle.GetCampsByStageId(1);
            var stageResult = stage.GetValue();                        
            
            //關卡初始化, 產生角色, 定位
            
            //拿整場戰鬥的結果
            var fight = battle.ToFight(1);            
                
            var battleResult = fight.GetValue();

            //BattleStatus.Success贏  BattleStatus.Fail 輸
            if (battleResult != null && battleResult.Status == BattleStatus.Success)
            {                
                var actions = battleResult.Actions;                

                for (int i = 0; i < actions.Length; ++i)
                {
                    var action = actions[i];
                    
                    var handle = new BindingHandle();
                    
                    handle.SetReferenceObject(this);
                    
                    var state = new BattleActState(action.SkillId.ToString(), action);
                    
                    state.AddBehaviour(TimelienBehaviour.Create(handle));                                        
                        
                   // handle.SetReferenceObject(this);
                    
                    //_StateMachine.AddState(new BattleActState(action.SkillId.ToString(), handle, action));    
                }                               
            }
        }

        private void Start()
        {
            //_StateMachine.AddState();
        }

        public Role GetRole(string id)
        {
            return null;
        }

        public CameraUnit GetCamera(string id)
        {
            return null;
        }

        public PlayableDirector GetPlayableDirector(string skillId)
        {
            return null;
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