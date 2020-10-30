using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TP.Scene.Locators.Editor
{
    [CustomEditor(typeof(GroupLocator)), CanEditMultipleObjects]
    public class GroupLocatorEditor : UnityEditor.Editor
    {
        static List<GroupLocator> _Locators = new List<GroupLocator>();
        
        [DrawGizmo(GizmoType.NonSelected | GizmoType.Active)]
        static void OnDrawGizmos(GroupLocator locator, GizmoType gizmoType)
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
                Drawing(locator);
            }
        }

        private static void Drawing(GroupLocator locator)
        {
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
            
            UnityEditor.Handles.Label(labelPos, "Group: " + locator.Index, new GUIStyle(){fontStyle = FontStyle.Bold, fontSize = font});
            
            UnityEditor.Handles.EndGUI();                      
            
            if (Handles.Button(showPos,  Quaternion.LookRotation(Vector3.down), locator.ShowSize,
                locator.ShowSize * 1.2f , Handles.ConeHandleCap))
            {
                Selection.activeObject = locator;
            }
        }      
    }
}