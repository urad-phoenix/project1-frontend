using UnityEngine;

namespace Phoenix.Playables.Editors
{    
    using UnityEditor;
    
    [CustomEditor(typeof(SpineAnimationClip))]
    public class SpineAnimationClipInspector : Editor
    {
        private SpineAnimationClip m_AnimatorClip;

        private void OnEnable()
        {
            m_AnimatorClip = target as SpineAnimationClip;
        }

        public override void OnInspectorGUI()
        {
            if (m_AnimatorClip.Names == null || m_AnimatorClip.Names.Count == 0)
                return;

            var name = m_AnimatorClip.template.Name;
            
            var index = m_AnimatorClip.Names.FindIndex(x => x == name);

            if (index < 0)
                index = 0;
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("States", GUILayout.Width(100));
            index = EditorGUILayout.Popup(index, m_AnimatorClip.Names.ToArray());
            name = m_AnimatorClip.Names[index];                        
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Track", GUILayout.Width(100));
            var track = EditorGUILayout.IntField(m_AnimatorClip.template.Track);             
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Loop", GUILayout.Width(100));
            var loop = EditorGUILayout.Toggle(m_AnimatorClip.template.IsLoop);             
            GUILayout.EndHorizontal();    

            if (GUI.changed)
            {
                m_AnimatorClip.template.IsLoop = loop;
                m_AnimatorClip.template.Track = track;
                m_AnimatorClip.template.Name = name;
                m_AnimatorClip.SetAnimator(m_AnimatorClip, m_AnimatorClip.Animator);
                EditorUtility.SetDirty(m_AnimatorClip);
            }
        }
    }
}