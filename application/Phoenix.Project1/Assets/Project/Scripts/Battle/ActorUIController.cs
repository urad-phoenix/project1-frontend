using System.Collections.Generic;
using Phoenix.Pool;
using Phoenix.Project1.Common.Battles;
using Project.Scripts.UI;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class ActorUIController : MonoBehaviour
    {        
        public Canvas HUDCanvas;

        [SerializeField]
        private UIHUD HudSource;

        private List<UIHUD> _HUDList;

        private Camera _Camera;

        [SerializeField] 
        private TextJumpComponent[] _Texts;

        private List<string> _TextKey;
      
        public ActorUIController()
        {
            _HUDList = new List<UIHUD>();  
            _TextKey = new List<string>();
        }

        private void Start()
        {
            CreateTexts();
        }

        public UIHUD InstantiateHUD(Avatar avatar)
        {
            var hud = Instantiate(HudSource, HUDCanvas.transform);
                        
            hud.Binding(avatar.InstanceID, avatar.GetDummy(DummyType.Location.ToString()));
            
            hud.BindingCamera(_Camera);
            
            _HUDList.Add(hud);
            
            return hud;
        }

        private void CreateTexts()
        {
            if (!PoolManager.Instance)
            {
                return;
            }

            foreach (var text in _Texts)
            {
                var pool = new ObjectPool(text.name, text.gameObject, this.transform, 5);

                pool.OnAfterSpawn += _TextAfterSpawn;

                _TextKey.Add(text.name);
                
                pool.Initialize();
                
                pool.Spawn();

                PoolManager.Instance.AddPool(pool);
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