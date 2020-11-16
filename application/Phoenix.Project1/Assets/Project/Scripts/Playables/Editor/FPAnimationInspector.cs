namespace Phoenix.Playables.Editors
{
    using UnityEngine;
    using UnityEditor;
    using Phoenix.Playables;
    
    [CustomEditor(typeof(AnimationTrack))]
    public class AnimationInspector : Editor
    {
        private AnimationTrack m_Target;

        private void OnEnable()
        {
            m_Target = target as AnimationTrack;
        }

        public override void OnInspectorGUI()
        {                  
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Apply Avatar Mask", GUILayout.Width(100));
            m_Target.applyAvatarMask = EditorGUILayout.Toggle(m_Target.applyAvatarMask);     
            GUILayout.EndHorizontal();
       
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Avatar Mask", GUILayout.Width(100));
            m_Target.avatarMask = EditorGUILayout.ObjectField(m_Target.avatarMask, typeof(AvatarMask), false) as AvatarMask;
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BindingType", GUILayout.Width(100));
            m_Target.BindingType = (BindingTrackType)EditorGUILayout.EnumPopup(m_Target.BindingType);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("CategoryType", GUILayout.Width(100));
            m_Target.Category = (AnimationCategory)EditorGUILayout.EnumPopup(m_Target.Category);
            GUILayout.EndHorizontal();

            if (m_Target.Category == AnimationCategory.Camera)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Key", GUILayout.Width(100));
                m_Target.Key = EditorGUILayout.TextField(m_Target.Key);
                GUILayout.EndHorizontal();    
            }
            
            if (GUI.changed)
            {                
                EditorUtility.SetDirty(target);
            }
        }
    }
}

