
/*namespace Phoenix.Playables.Editor
{
    using UnityEditor;
    using UnityEngine;
    using Phoenix.Playables;

    [CustomEditor(typeof(EventTrack))]
    public class EventTrackInspector : Editor
    {
        private EventTrack m_Track;
        
        private void OnEnable()
        {
            m_Track = target as EventTrack;
            Undo.undoRedoPerformed += UndoCallback;
        }
        
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Event Type", GUILayout.Width(100));
            var eventType = (FantasyPuzzles.Playables.EventType)EditorGUILayout.EnumPopup(m_Track.EventType);          
            GUILayout.EndHorizontal();
            
            if (EditorGUI.EndChangeCheck())
            {           
                Undo.RecordObject(m_Track,  "EventTrack");
                              
                if (m_Track.EventType != eventType)
                {         
                    m_Track.ChangeClip(eventType);
                }   
                
                m_Track.EventType = eventType;
                               
                EditorUtility.SetDirty(m_Track);
            }
        }

        public void UndoCallback()
        {
            m_Track.ChangeClip(m_Track.EventType);
        }
    }
}*/