#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TP.Scene.Locators
{
    public class CampLocator : MonoBehaviour
    {
        public int CampType;
        
        #if UNITY_EDITOR
        
        [Header("Scene Editor")]
        [SerializeField]
        public Color Color = Color.blue;

        [SerializeField]
        public float ShownPos = 1.8f;
        
        [SerializeField]
        public float LabelShownPos = 2.3f;

        [SerializeField] 
        public int FontMaxSize = 10;
        
        [SerializeField] 
        public int FontMinSize = 3;

        [SerializeField] 
        public float ShowSize = 0.3f;
        
        private void OnDrawGizmos()
        {
//            if (_Locator == null)
//            {
//                _Locator = this;
//                SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
//            }
//            
//            CampLocator locator = this;         
//                        
//            Handles.color = locator._Color;                        
//            
//            UnityEditor.Handles.BeginGUI();
//
//            var zoom = UnityEditor.SceneView.currentDrawingSceneView.camera.orthographicSize;
//
//            var size = Mathf.RoundToInt(locator._FontMaxSize * zoom);
//            
//            size = size > locator._FontMaxSize ? locator._FontMaxSize : 
//                size < locator._FontMinSize ? locator._FontMinSize : size;
//            
//            UnityEditor.Handles.color = locator._Color;
//           
//
//            UnityEditor.Handles.Label(locator.transform.position + Vector3.up * locator._LabelShownPos, "Camp: " + locator.CampType, new GUIStyle(){fontSize = size});
//            
//            UnityEditor.Handles.EndGUI();
//            
//            
//            Handles.ConeHandleCap(0, locator.transform.position + new Vector3(0f, _ShownPos, 0f),
//                Quaternion.LookRotation(Vector3.down), _ShowRadius, EventType.Repaint);

  


        }    
        
        #endif
    }
   
}