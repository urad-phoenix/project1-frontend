using System;
using System.Collections.Generic;
using System.Linq;
using Phoenix.Pool;
using Phoenix.Project1.Common.Battles;
using Project.Scripts.UI;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class ActorUIController : MonoBehaviour
    {        
        [Serializable]
        internal class TextData
        {
            public EffectType Key;

            public TextJumpComponent TextJump;
        }
        
        public Canvas HUDCanvas;

        [SerializeField]
        private UIHUD HudSource;

        private List<UIHUD> _HUDList;

        private Camera _Camera;

        [SerializeField] 
        private TextData[] _Texts;

        private List<string> _TextKey;
      
        public ActorUIController()
        {
            _HUDList = new List<UIHUD>();  
            _TextKey = new List<string>();
        }

        public UIHUD InstantiateHUD(Avatar avatar)
        {
            var hud = Instantiate(HudSource, HUDCanvas.transform);
                        
            hud.Binding(avatar.InstanceID, avatar.GetDummy(DummyType.Location.ToString()));
            
            hud.BindingCamera(_Camera);
            
            _HUDList.Add(hud);
            
            return hud;
        }

        public void CreateTexts()
        {
            if (!PoolManager.Instance)
            {
                return;
            }

            foreach (var text in _Texts)
            {
                var key = text.Key.ToString();
                
                text.TextJump.gameObject.name = key;
                
                Debug.Log($"text key {key}");
                
                var pool = new ObjectPool(key, text.TextJump.gameObject, this.transform, 5);

                pool.OnAfterSpawn += _TextAfterSpawn;

                _TextKey.Add(key);

                PoolManager.Instance.AddPool(pool);
                
                pool.Spawn();
            }                        
        }

        private void _TextAfterSpawn(GameObject go)
        {
            var component = go.GetComponent<TextJumpComponent>();

            var obs = component.RegisterCompleteCallback();

            obs.Subscribe(_RecycleText).AddTo(go);
        }

        private void _RecycleText(GameObject go)
        {            
            //Debug.Log($"_RecycleText {go.name}");
            
            var key = _TextKey.Find(x => go.name.Contains(x));

            if (!PoolManager.Instance.IsContainsPool(key))
            {
                Destroy(go);
                return;
            }
            
            PoolManager.Instance.Recycle(key, go);
        }

        public void SettingCamera(Camera camera)
        {
            _Camera = camera;
            
            foreach (var uihud in _HUDList)
            {
                uihud.BindingCamera(camera);
            }
        }

        public void ShowJumpText(EffectType type, string text, Avatar avatar, bool isAttacker)
        {         
            var textObj = PoolManager.Instance.GetObject<GameObject>(type.ToString());

            textObj.transform.position = _GetPosition(_Camera, avatar.GetDummy(DummyType.UIText.ToString()));
            
            var component = textObj.GetComponent<TextJumpComponent>();

            component.InvertEnd = isAttacker;

            component.SetTextJumpAsObservable(text);
            var obs = from jump in component.SetTextJumpAsObservable(text)
                select jump;

            obs.Subscribe().AddTo(gameObject);
            //obs.Subscribe(unit => _Compelete()).AddTo(gameObject);

//            var go = _Texts.First(x => x.Key == type);
//
//            var textObj = Instantiate(go.TextJump.gameObject, this.transform);
//            
//            textObj.transform.position = _GetPosition(_Camera, avatar.GetDummy(DummyType.UIText.ToString()));
//            
//            textObj.SetActive(true);
//            
//            var component = textObj.GetComponent<TextJumpComponent>();
//
//            component.IsAutoDestroy = true;
//            
//            component.InvertEnd = isAttacker;
//
//            component.SetTexture(text, _GetPosition(_Camera, avatar.GetDummy(DummyType.UIText.ToString())));
            
            
        }                
        
        private Vector3 _GetPosition(Camera camera, Transform follow)
        {
           if(!camera || !follow)
               return Vector3.zero;
            
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, follow.position);
            
            return screenPoint;                                  
        }

//        private void _Compelete()
//        {
//            //Debug.Log($"_Compelete");
//        }

//        public void SetCurrentBlood(int id, int value)
//        {
//            Debug.Log($"SetCurrentBlood : {id}, {value}");
//            if (_HUDList == null)
//                return;
//            
//            var hud = _HUDList.Find(x => x.FollowId == id);
//            
//            hud?.SetCurrentBlood(value);
//        }
//        
//        public void SetHUDActive(int id, bool isActive)
//        {
//            var hud = _HUDList.Find(x => x.FollowId == id);
//            
//            hud?.gameObject.SetActive(isActive);
//        }
//
//        public void ReduceHP(int id, int value)
//        {
//            var hud = _HUDList.Find(x => x.FollowId == id);
//            
//            hud.ReduceBlood(value);
//        }
//        
//        public void IncreaseHP(int id, int value)
//        {
//            
//        }
    }
}