using System.Linq;
using UnityEngine;

namespace Phoenix.Playables.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(AnimatorClip))]
    public class AnimatorInspector : Editor
    {
        private AnimatorClip m_AnimatorClip;

        private void OnEnable()
        {
            m_AnimatorClip = target as AnimatorClip;
        }

        public override void OnInspectorGUI()
        {
            if (m_AnimatorClip.Names == null || m_AnimatorClip.Names.Count == 0)
                return;
            
            var index = m_AnimatorClip.Names.FindIndex(x => x == m_AnimatorClip.StateKey);

            if (index < 0)
                index = 0;
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("States", GUILayout.Width(100));
            index = EditorGUILayout.Popup(index, m_AnimatorClip.Names.ToArray());
            m_AnimatorClip.StateKey = m_AnimatorClip.Names[index];                        
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Layer", GUILayout.Width(100));
            m_AnimatorClip.Layer = EditorGUILayout.IntField(m_AnimatorClip.Layer);             
            GUILayout.EndHorizontal();        

            if (GUI.changed)
            {
                m_AnimatorClip.SetAnimator(m_AnimatorClip, m_AnimatorClip.Animator, m_AnimatorClip.TimeClip);
                EditorUtility.SetDirty(m_AnimatorClip);
            }
        }
    }    
}

