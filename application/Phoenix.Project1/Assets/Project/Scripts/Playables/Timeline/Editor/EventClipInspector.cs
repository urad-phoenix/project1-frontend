/*namespace Phoenix.Playables.Editor
{
    using UnityEngine;
    using UnityEditor;
    using FPTimeline;
    
    [CustomEditor(typeof(EventClip))]
    public class EventClipInspector : Editor
    {
        private EventClip m_Clip;
        
        private void OnEnable()
        {
            m_Clip = target as EventClip;
          
        }
       
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal(GUILayout.Width(10));            
            EditorGUILayout.LabelField("Event Type", GUILayout.Width(100));
            
            //var eventType = (FPTimeline.Structs.Enum.EventType)EditorGUILayout.EnumPopup(m_Clip.EventType);
            EditorGUILayout.LabelField(m_Clip.EventType.ToString(), EditorStyles.helpBox, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            if (m_Clip.Event != null)// && m_Clip.Event.EventType == m_Clip.EventType)
            {
                var fields = m_Clip.Event.GetType().GetFields();
                for (int i = 0; i < fields.Length; ++i)
                {
                    var value = EidtorUtility.DrawObjectField(fields[i].Name, fields[i], fields[i].GetValue(m_Clip.Event), 100);

                    if (value != null)
                    {
                        fields[i].SetValue(m_Clip.Event, value);
                    }
                }
            }
            else
            {
               // Undo.RecordObject(m_Clip,  "EventClip");
                m_Clip.CreateEvent();
                m_Clip.Serializer();
                //EditorUtility.SetDirty(m_Clip); 
            }
            
            if (EditorGUI.EndChangeCheck())
            {                                     
                Undo.RecordObject(m_Clip,  "EventClip");
                //m_Clip.EventType = eventType;
                if (m_Clip.Event.EventType != m_Clip.EventType)
                {                    
                    m_Clip.CreateEvent();                                       
                }   
                
                m_Clip.Serializer();
                EditorUtility.SetDirty(m_Clip);
            }
        }       
    }    
}*/
