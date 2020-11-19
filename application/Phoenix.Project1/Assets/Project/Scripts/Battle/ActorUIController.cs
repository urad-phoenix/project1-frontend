using System.Collections.Generic;
using Phoenix.Project1.Common.Battles;
using Project.Scripts.UI;
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

      
        public ActorUIController()
        {
            _HUDList = new List<UIHUD>();                        
        }

        public UIHUD InstantiateHUD(Avatar avatar)
        {
            var hud = Instantiate(HudSource, HUDCanvas.transform);
                        
            hud.Binding(avatar.InstanceID, avatar.GetDummy(DummyType.Location.ToString()));
            
            hud.BindingCamera(_Camera);
            
            _HUDList.Add(hud);

            return hud;
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