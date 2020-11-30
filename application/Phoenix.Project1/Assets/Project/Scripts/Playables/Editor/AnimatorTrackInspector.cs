using System.Collections.Generic;
using Phoenix.Playables;
using UnityEngine;

namespace Playables.Timeline.Editors
{
    using UnityEditor;
    
    [CustomEditor(typeof(AnimatorTrack))]
    public class AnimatorTrackInspector : Editor
    {
        private AnimatorTrack m_Track;

        private List<string> m_States = new List<string>();

        private void OnEnable()
        {
            m_Track = target as AnimatorTrack;
            SetAnimatorStateNames();
        }

        private void SetAnimatorStateNames()
        {
            m_States.Clear();
            foreach (var clip in m_Track.GetClips())
            {
                var c = clip.asset as AnimatorClip;
                m_States.AddRange(c.Names);
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BindingType", GUILayout.Width(100));
            m_Track.BindingType = (BindingTrackType) EditorGUILayout.EnumPopup(m_Track.BindingType);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                m_Track.IsReturnToSpecifyState =
                    EditorGUILayout.Toggle("IsReturnToSpecifyState", m_Track.IsReturnToSpecifyState);

                if (m_Track.IsReturnToSpecifyState)
                {
                    var returnIndex = m_States.FindIndex(x => x == m_Track.ReturnKey);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("ReturnKey", GUILayout.Width(100));
                    returnIndex = EditorGUILayout.Popup(returnIndex, m_States.ToArray());
                    m_Track.ReturnKey = m_States[returnIndex];
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            
            if (GUI.changed)
            {
                SetAnimatorStateNames();
            }
        }
    }
}