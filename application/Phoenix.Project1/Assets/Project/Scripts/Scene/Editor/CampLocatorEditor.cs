using System.Collections.Generic;
using UnityEngine;

namespace TP.Scene.Locators.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(CampLocator)), CanEditMultipleObjects]    
    public class CampLocatorEditor : Editor
    {        
        static List<CampLocator> _Locators = new List<CampLocator>();

        private CampLocator _CampLocator;
        
        private void OnEnable()
        {
            _CampLocator = target as CampLocator;                        
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUI.changed)
            {
                var characters = _CampLocator.GetComponentsInChildren<CharacterLocator>();

                foreach (var characterLocator in characters)
                {
                    characterLocator.CampType = _CampLocator.CampType;
                }
            }
        }
        
        
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Active)]
        static void OnDrawGizmos(CampLocator locator, GizmoType gizmoType)
        {
            if (!_Locators.Exists(x => x == locator))
            {
                if (_Locators.Count == 0)
                {
                    SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
                }

                _Locators.Add(locator);                
            }
            
            if(!locator.gameObject.activeSelf)
            {
                _Locators.Remove(locator);                                
            }

            if (_Locators.Count == 0)
            {
                SceneView.onSceneGUIDelegate += OnSceneGuiDelegate;
            }
        }
                
        private static void OnSceneGuiDelegate(SceneView sceneview)
        {
            for (int i = 0; i < _Locators.Count; ++i)
            {
                var locator = _Locators[i];

                if (locator == null)
                {
                    _Locators.RemoveAt(i);
                    
                    return;
                }
                Drawing(locator);
            }
        }

        public static void Drawing(CampLocator locator)
        {
            if(locator == null)
                return;
            
            Handles.color = locator.Color;                        
            
            UnityEditor.Handles.BeginGUI();

            var zoom = UnityEditor.SceneView.currentDrawingSceneView.camera.orthographicSize;

            var font = Mathf.RoundToInt(locator.FontMaxSize * zoom);
            
            font = font > locator.FontMaxSize ? locator.FontMaxSize : 
                font < locator.FontMinSize ? locator.FontMinSize : font;
            
            UnityEditor.Handles.color = locator.Color;
            
            var showPos = locator.transform.position + Vector3.up * locator.ShownPos;
            
            var labelPos = locator.transform.position + Vector3.up * locator.LabelShownPos;

            UnityEditor.Handles.color = locator.Color;
            
            UnityEditor.Handles.Label(labelPos, "Camp: " + (int) locator.CampType, new GUIStyle(){fontStyle = FontStyle.Bold, fontSize = font});
            
            UnityEditor.Handles.EndGUI();
           
            Handles.ArrowHandleCap(locator.GetInstanceID(),showPos, locator.transform.rotation, locator.ShowSize, EventType.Repaint);
            
            if (Handles.Button(showPos,  Quaternion.LookRotation(Vector3.down), locator.ShowSize,
                locator.ShowSize * 1.2f , Handles.ConeHandleCap))
            {
                Selection.activeObject = locator;
            }
        }       
    }
}