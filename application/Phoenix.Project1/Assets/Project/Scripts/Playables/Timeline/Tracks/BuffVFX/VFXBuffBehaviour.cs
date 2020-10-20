using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables
{    
    public class VFXBuffData : PlayableBehaviour
    {
        public bool IsFirstFrameHappened;
        public string Key;
        public bool IsRestart;
    }
    
    public class VFXBuffBehaviour : PlayableBehaviour
    {
        private GameObject VFX;
        private string m_Key;
        private bool m_IsRestart;
        private double m_Time;
        private bool m_IsClipPlaying;        
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            VFX = playerData as GameObject;
            
            if(VFX == null)
                return;

            var count = playable.GetInputCount();

            if (m_IsRestart && m_IsClipPlaying)
            {
                m_Time -= info.deltaTime;

                if (m_Time < 0.0f)
                {
                    m_IsClipPlaying = false;
                }
            }

            for (int i = 0; i < count; ++i)
            {
                var input = (ScriptPlayable<VFXBuffData>)playable.GetInput(i);

                var weight = playable.GetInputWeight(i);

                var data = input.GetBehaviour();
                                
                if (weight > 0)
                {
                    if (m_IsRestart && !m_IsClipPlaying)
                        data.IsFirstFrameHappened = false;
                    
                    if (!data.IsFirstFrameHappened)
                    {
                        m_IsClipPlaying = true;
                        m_Key = data.Key;
                        data.IsFirstFrameHappened = true;
                        m_Time = input.GetDuration();

                        m_IsRestart = data.IsRestart;
                        if (data.IsRestart)
                        {
                            if(VFX.activeSelf)
                                VFX.SetActive(false);
                        }

                        if(!VFX.activeSelf)
                            VFX.SetActive(true);    
                    }                    
                }
                else
                {
                    if (data.IsRestart)
                    {
                        m_IsClipPlaying = false;
                        data.IsFirstFrameHappened = false;
                        if(VFX.activeSelf)
                            VFX.SetActive(false);                        
                    }
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (VFX != null)
            {
                if(VFX.activeSelf)
                    VFX.SetActive(false);
                
                //if (Application.isPlaying && Entry.IsInitialized)
                //{
                //    var poolManager = Entry.GetModule<PoolManager>();
                //    VFX.transform.parent = null;
                //    poolManager.Recycle(m_Key, VFX);
                //}

                VFX = null;
            }
        }
    }
}