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
        
        private readonly UniRx.CompositeDisposable _SendDisposables;
        
        private readonly UniRx.CompositeDisposable _SendRequestDisposables;     
        
        public BattleController()
        {
            _StateMachine = new BattleStateMachine();                        
            
            _disposable = new CompositeDisposable();
            
            _SendDisposables = new CompositeDisposable();
            
            _SendRequestDisposables = new CompositeDisposable();
        }

        void Start()
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattle>()
                select battle;

            battleObs.Subscribe(_GetBattle).AddTo(_disposable);
        } 
        
        private void _EndBattle()
        {
            var battleObs = from battle in NotifierRx.ToObservable().Supply<IBattle>()
                select battle;
            
            battleObs.Subscribe(_ExitBattle).AddTo(_SendDisposables); 
        }              

        private void _ExitBattle(IBattle battle)
        {
            battle.Exit();
        }    

        private void _GetBattle(IBattle battle)
        {            
            //TODO
            //關卡資訊, 之後要移到關卡腳本
            var stage = battle.GetCampsByStageId(1);
            
            var stageResult = stage.GetValue();

            if (stageResult == null)
            {                
                _EndBattle();
                return;
            }
            //關卡初始化, 產生角色, 定位
            
            //拿整場戰鬥的結果
            var fight = battle.ToFight(1);            
                
            var battleResult = fight.GetValue();

            //BattleStatus.Success贏  BattleStatus.Fail 輸
            if (battleResult != null)
            {                
                var actions = battleResult.Actions;                

                for (int i = 0; i < actions.Length; ++i)
                {
                    var action = actions[i];
                    
                    var handle = new BindingHandle();
                    
                    handle.SetReferenceObject(this);

                    var binding = new StateBinding(handle);
                    
                    var state = new BattleActState(action.SkillId.ToString(), action, binding);
                   
                    _StateMachine.AddState(state);
                }

                _SetBattleFinished();
            }                        
        }

        void _SetBattleFinished()
        {
            //TODO
            //跟server要獎勵或是第一次同步就已經同步獎勵完成
            _StateMachine.AddState(new BattleResultState("finished"));
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

        public void PlaySound(string key)
        {
        }                

        public void RecyclePlayableDirector(PlayableDirector playableDirector)
        {
            //playableDirector.re
        }

        private void _Finished()
        {
            _disposable.Clear();
            
            _SendDisposables.Clear();
            
            _SendRequestDisposables.Clear();
        }

        private void OnDestroy()
        {
            _Finished();
        }
    }
}