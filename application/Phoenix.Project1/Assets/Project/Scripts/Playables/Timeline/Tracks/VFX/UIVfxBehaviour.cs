using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables
{
    public class UIVfxBehaviour : PlayableBehaviour
    {
        private string m_VFXKey;
        public GameObject VFX;
        private bool m_IsFirstFrameHappened;
        private float m_Time;
        private bool m_IsPlaying;
        public Vector3 StartPosition;
        public Vector3 TargetPosition;

        public void Binding(string key, GameObject vfxObj, float time, Vector3 startPosition, Vector3 targetPosition)
        {
            m_VFXKey = key;
            VFX = vfxObj;         
            m_IsFirstFrameHappened = false;
            m_Time = time;
            m_IsPlaying = false;
            StartPosition = startPosition;
            TargetPosition = targetPosition;
            VFX.transform.position = startPosition;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            if (!m_IsFirstFrameHappened)
            {                
                m_IsPlaying = true;
                m_IsFirstFrameHappened = true;
            }
            
            if(!m_IsPlaying)
                return;
            
             m_Time -= info.deltaTime;
            
            if (m_Time < 0.0f)
            {
                var graph = playable.GetGraph();            
                graph.Stop();   
            }
            
            Debug.Log("UIVfxBehaviour frame");
        }       

        public override void OnGraphStop(Playable playable)
        {
            base.OnGraphStop(playable);
            VFX = null;
            m_VFXKey = null;
            Debug.Log("UIVfxBehaviour destroy");          
        }
    }
}