using System.Collections.Generic;
using UnityEngine;

namespace TP.Scene.Locators.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(CharacterLocator)), CanEditMultipleObjects]
    public class CharacterLocatorEditor : Editor
    {
         static List<CharacterLocator> _Locators = new List<CharacterLocator>();

        private void OnSceneGUI()
        {
            var locator = (CharacterLocator) target;
            
            var camp = locator.transform.GetComponentInParent<CampLocator>();
            
            var zoom = UnityEditor.SceneView.currentDrawingSceneView.camera.orthographicSize;
            
            var font = Mathf.RoundToInt(locator.FontMaxSize * zoom);                        
            
            font = font > locator.FontMaxSize ? locator.FontMaxSize : 
                font < locator.FontMinSize ? locator.FontMinSize : font;       
            
            var labelPos = locator.transform.position + Vector3.up * locator.LabelShownPos;
            locator.CampType = camp.CampType;
            UnityEditor.Handles.Label(labelPos, "Camp: " + (camp ? camp.CampType.ToString() : "have not in camp") + " Index: " + locator.Index, new GUIStyle(){fontSize = font});
            locator.Color = camp ? camp.Color : locator.Color;
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Active)]
        static void OnDrawGizmos(CharacterLocator locator, GizmoType gizmoType)
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

        private static void Drawing(CharacterLocator locator)
        {           
            Handles.BeginGUI();
           
            Handles.EndGUI();
            
            Handles.color = locator.Color;                                                           
            
            UnityEditor.Handles.color = locator.Color;           
            
//            Handles.ConeHandleCap(locator.GetInstanceID(), locator.transform.position,
//                locator.transform.rotation, locator._ShowRadius, EventType.Repaint);                                   

            var camp = locator.transform.GetComponentInParent<CampLocator>();
            locator.Color = camp ? camp.Color : locator.Color;
            
            if (Handles.Button(locator.transform.position, locator.transform.rotation, locator.ShowSize,
                locator.ShowSize * 1.2f , Handles.ConeHandleCap))
            {                                         
                Selection.activeObject = locator;
            }
        }
    }
}