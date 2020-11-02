using System.Linq;
using UnityEditor.SceneManagement;

namespace Phoenix.Playables.Editor
{
    using System;
    using Cinemachine.Editor;
    using UnityEditor;
    using UnityEngine;    
    using System.Collections.Generic;
    using Cinemachine;
    
    [CustomEditor(typeof(CameraShotClip))]
    internal sealed class CameraShotInspector : BaseEditor<CameraShotClip>
    {
        private static readonly GUIContent kVirtualCameraLabel
            = new GUIContent("Virtual Camera", "The virtual camera to use for this shot");

        protected override List<string> GetExcludedPropertiesInInspector()
        {
            List<string> excluded = base.GetExcludedPropertiesInInspector();
            excluded.Add(FieldPath(x => x.VirtualCamera));
            return excluded;
        }

        private void OnDisable()
        {
            DestroyComponentEditors();
        }

        private void OnDestroy()
        {
            DestroyComponentEditors();
        }

        private CameraShotClip m_ShotClip;
        
        private void OnEnable()
        {
            m_ShotClip = target as CameraShotClip;  
            RebuildProfileList();
        }     
        
        private List<ScriptableObject> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<ScriptableObject> assets = new List<ScriptableObject>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                ScriptableObject asset = AssetDatabase.LoadAssetAtPath<T>(assetPath) as ScriptableObject;
                if (asset != null)
                    assets.Add(asset);
            }
            return assets;
        }               

        private List<GUIContent> presetNameList = new List<GUIContent>();
        
        private List<ScriptableObject> noiseSettings = new List<ScriptableObject>();

        private void RebuildProfileList()
        {           
            InvalidateProfileList();
            
            noiseSettings = FindAssetsByType<NoiseSettings>();
                        
            noiseSettings.Insert(0, null);
            
            for (int i = 0; i < noiseSettings.Count; ++i)
            {
                presetNameList.Add(new GUIContent(noiseSettings[i] == null ? "(none)" : noiseSettings[i].name));
            }
        }
        
        private void InvalidateProfileList()
        {
            presetNameList.Clear();
            noiseSettings.Clear();
        }

        public override void OnInspectorGUI()
        {
            BeginInspector();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var isOpenNoise = EditorGUILayout.Toggle("IsOpenNoise", m_ShotClip.IsOpenNoise);
            int preset = -1;
            float amplitude = 0;
            float frequency = 0;
            if (isOpenNoise)
            {
                preset = noiseSettings.IndexOf(m_ShotClip.CameraNoiseSetting.NoiseSettings);
                preset = EditorGUILayout.Popup(new GUIContent("NoiseSetting"), preset, presetNameList.ToArray());
                amplitude = EditorGUILayout.FloatField("AmplitudeGain", m_ShotClip.CameraNoiseSetting.AmplitudeGain);
                frequency = EditorGUILayout.FloatField("FrequencyGain", m_ShotClip.CameraNoiseSetting.FrequencyGain);
            }

            if (GUI.changed)
            {
                //Undo.RecordObject(m_ShotClip,  "ShotClip");                               
                m_ShotClip.IsOpenNoise = isOpenNoise;
                m_ShotClip.CameraNoiseSetting.NoiseSettings = preset < 0 || preset >= noiseSettings.Count ? null : noiseSettings[preset] as NoiseSettings;
                m_ShotClip.CameraNoiseSetting.AmplitudeGain = amplitude;
                m_ShotClip.CameraNoiseSetting.FrequencyGain = frequency;
                //EditorUtility.SetDirty(m_ShotClip);
                //EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            EditorGUILayout.EndVertical();
            
            SerializedProperty vcamProperty = FindProperty(x => x.VirtualCamera);
            EditorGUI.indentLevel = 0; // otherwise subeditor layouts get screwed up

            Rect rect;
            CinemachineVirtualCameraBase vcam
                = vcamProperty.exposedReferenceValue as CinemachineVirtualCameraBase;
            if (vcam != null)
                EditorGUILayout.PropertyField(vcamProperty, kVirtualCameraLabel);
            else
            {
                GUIContent createLabel = new GUIContent("Create");
                Vector2 createSize = GUI.skin.button.CalcSize(createLabel);

                rect = EditorGUILayout.GetControlRect(true);
                rect.width -= createSize.x;

                EditorGUI.PropertyField(rect, vcamProperty, kVirtualCameraLabel);
                rect.x += rect.width; rect.width = createSize.x;
                if (GUI.Button(rect, createLabel))
                {
                    vcam = CreateDefaultVirtualCamera();
                    vcamProperty.exposedReferenceValue = vcam;
                }
                serializedObject.ApplyModifiedProperties();
            }

            DrawRemainingPropertiesInInspector();

            if (vcam != null)
                DrawSubeditors(vcam);
        }

        void DrawSubeditors(CinemachineVirtualCameraBase vcam)
        {
            // Create an editor for each of the cinemachine virtual cam and its components
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
            UpdateComponentEditors(vcam);
            if (m_editors != null)
            {
                foreach (UnityEditor.Editor e in m_editors)
                {
                    if (e == null || e.target == null || (e.target.hideFlags & HideFlags.HideInInspector) != 0)
                        continue;

                    // Separator line - how do you make a thinner one?
                    GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) } );

                    bool expanded = true;
                    if (!s_EditorExpanded.TryGetValue(e.target.GetType(), out expanded))
                        expanded = true;
                    expanded = EditorGUILayout.Foldout(
                        expanded, e.target.GetType().Name, true, foldoutStyle);
                    if (expanded)
                        e.OnInspectorGUI();
                    s_EditorExpanded[e.target.GetType()] = expanded;
                }
            }
        }

        CinemachineVirtualCameraBase m_cachedReferenceObject;
        UnityEditor.Editor[] m_editors = null;
        static Dictionary<System.Type, bool> s_EditorExpanded = new Dictionary<System.Type, bool>();

        void UpdateComponentEditors(CinemachineVirtualCameraBase obj)
        {
            MonoBehaviour[] components = null;
            if (obj != null)
                components = obj.gameObject.GetComponents<MonoBehaviour>();
            int numComponents = (components == null) ? 0 : components.Length;
            int numEditors = (m_editors == null) ? 0 : m_editors.Length;
            if (m_cachedReferenceObject != obj || (numComponents + 1) != numEditors)
            {
                DestroyComponentEditors();
                m_cachedReferenceObject = obj;
                if (obj != null)
                {
                    m_editors = new UnityEditor.Editor[components.Length + 1];
                    CreateCachedEditor(obj.gameObject.GetComponent<Transform>(), null, ref m_editors[0]);
                    for (int i = 0; i < components.Length; ++i)
                        CreateCachedEditor(components[i], null, ref m_editors[i + 1]);
                }
            }
        }

        void DestroyComponentEditors()
        {
            m_cachedReferenceObject = null;
            if (m_editors != null)
            {
                for (int i = 0; i < m_editors.Length; ++i)
                {
                    if (m_editors[i] != null)
                        UnityEngine.Object.DestroyImmediate(m_editors[i]);
                    m_editors[i] = null;
                }
                m_editors = null;
            }
        }
        
        public static CinemachineVirtualCamera CreateDefaultVirtualCamera()
        {
            return InternalCreateVirtualCamera(
                "CM vcam", false, typeof(CinemachineComposer), typeof(CinemachineTransposer));
        }
        
        static CinemachineVirtualCamera InternalCreateVirtualCamera(
            string name, bool selectIt, params Type[] components)
        {
            // Create a new virtual camera
            CreateCameraBrainIfAbsent();
            GameObject go = CreateGameObject(
                GenerateUniqueObjectName(typeof(CinemachineVirtualCamera), name),
                typeof(CinemachineVirtualCamera));
            if (SceneView.lastActiveSceneView != null)
                go.transform.position = SceneView.lastActiveSceneView.pivot;
            Undo.RegisterCreatedObjectUndo(go, "create " + name);
            CinemachineVirtualCamera vcam = go.GetComponent<CinemachineVirtualCamera>();
            GameObject componentOwner = vcam.GetComponentOwner().gameObject;
            foreach (Type t in components)
                Undo.AddComponent(componentOwner, t);
            vcam.InvalidateComponentPipeline();
            if (selectIt)
                Selection.activeObject = go;
            return vcam;
        }
        
        /// <summary>
        /// If there is no CinemachineBrain in the scene, try to create one on the main camera
        /// </summary>
        public static void CreateCameraBrainIfAbsent()
        {
            CinemachineBrain[] brains = UnityEngine.Object.FindObjectsOfType(
                typeof(CinemachineBrain)) as CinemachineBrain[];
            if (brains == null || brains.Length == 0)
            {
                Camera cam = Camera.main;
                if (cam == null)
                {
                    Camera[] cams = UnityEngine.Object.FindObjectsOfType(
                        typeof(Camera)) as Camera[];
                    if (cams != null && cams.Length > 0)
                        cam = cams[0];
                }
                if (cam != null)
                {
                    Undo.AddComponent<CinemachineBrain>(cam.gameObject);
                }
            }
        }

        /// <summary>
        /// Generate a unique name with the given prefix by adding a suffix to it
        /// </summary>
        public static string GenerateUniqueObjectName(Type type, string prefix)
        {
            int count = 0;
            UnityEngine.Object[] all = Resources.FindObjectsOfTypeAll(type);
            foreach (UnityEngine.Object o in all)
            {
                if (o != null && o.name.StartsWith(prefix))
                {
                    string suffix = o.name.Substring(prefix.Length);
                    int i;
                    if (Int32.TryParse(suffix, out i) && i > count)
                        count = i;
                }
            }
            return prefix + (count + 1);
        }
        
        // Temporarily here
        public static GameObject CreateGameObject(string name, params Type[] types)
        {
#if UNITY_2018_3_OR_NEWER
            return ObjectFactory.CreateGameObject(name, types);
#else
            return new GameObject(name, types);
#endif
        }
    }
}
