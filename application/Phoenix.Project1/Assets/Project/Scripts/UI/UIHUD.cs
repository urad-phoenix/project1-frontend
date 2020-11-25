using System;
using System.Collections.Generic;
using Phoenix.Project1.Client;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote.Reactive;
using UniRx;
using UnityEngine;


namespace Project.Scripts.UI
{
    public class UIHUD : MonoBehaviour
    {                
        [SerializeField]
        private Transform _Follow;

        [SerializeField] 
        private Camera _FollowCamera;

        [SerializeField]
        private UIBlood _UIBlood;

        public int FollowId;
        
        private CompositeDisposable _Disposables;

        public UIHUD()
        {            
            _Disposables = new CompositeDisposable();
        }

        public void Binding(int id, Transform follow)
        {
            _Follow = follow;

            FollowId = id;

            gameObject.SetActive(true);
            
            var actorHpObs =  from battle in NotifierRx.ToObservable().Supply<IBattle>()
                from actor in battle.Actors.SupplyEvent()
                from newHp in actor.Hp.ChangeObservable()
                select new { actor, newHp };
            
            actorHpObs.DoOnError(_Error).ObserveOnMainThread()
                .Subscribe(v => _SetActorHp(v.actor.InstanceId.Value, v.newHp)).AddTo(_Disposables);
        }
        
        public void _Error(Exception exception)
        {
            Debug.LogError(exception);   
        }

        public void BindingCamera(Camera followCamera)
        {
            _FollowCamera = followCamera;
        }

        private void Start()
        {
            Observable.EveryUpdate().Subscribe(obs => _SetFollow()).AddTo(_Disposables);
        }

        private void _SetFollow()
        {
            if(_FollowCamera == null || _Follow == null)
                return;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(_FollowCamera, _Follow.position);
            
            transform.position = screenPoint;                                  
        }

        public void _SetActorHp(int id, float hp)
        {
            if (FollowId == id)
            {                
                _UIBlood.SetCurrentBlood((int)hp);
            }
        }    
        
        public void EffectTrigger(Effect effect)
        {
            //Debug.Log($"effect {effect.Type}  , hud id{FollowId}");

            if(effect.Actor != FollowId)
                return;
            
            if (effect.Type == EffectType.Heal)
            {
                _UIBlood.IncreaseValue(effect.Value);
            }
            else
            {
                _UIBlood.ReduceValue(effect.Value);
            }
        }
        
        private void OnDestroy()
        {
            _Disposables.Clear();
        }               
    }  
}