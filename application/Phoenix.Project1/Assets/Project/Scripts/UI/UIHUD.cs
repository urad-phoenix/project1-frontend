using System;
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
        
        public void Binding(int id, Transform follow)
        {
            _Follow = follow;

            FollowId = id;
            
            gameObject.SetActive(true);
        }

        public void BindingCamera(Camera followCamera)
        {
            _FollowCamera = followCamera;
        }

        private void Start()
        {
            Observable.EveryUpdate().Subscribe(obs => _SetFollow());
        }

        private void _SetFollow()
        {
            if(_FollowCamera == null || _Follow == null)
                return;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(_FollowCamera, _Follow.position);
            
            transform.position = screenPoint;                                  
        }

        public void SetCurrentBlood(int value)
        {
            _UIBlood.SetCurrentBlood(value);
        }

        public void ReduceBlood(int value)
        {
            _UIBlood.ReduceValue(value);
        }
        
        public void IncreaseBlood(int value)
        {
            _UIBlood.IncreaseValue(value);
        }
    }
}