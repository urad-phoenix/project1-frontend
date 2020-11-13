namespace Phoenix.Playables
{
    using System.Collections.Generic;
    using System.Reflection;
    using Cinemachine;
    using UnityEngine;
    using UnityEngine.Playables;
    
    public class CameraShotBehaviour : PlayableBehaviour
    {
         // The brain that this track controls
        private CinemachineBrain mBrain;
        private int mBrainOverrideId = -1;
        private bool mPlaying;
        private bool m_IsFirstFrameHappened;
        private MethodInfo m_Method;
        
        private List<CinemachineBasicMultiChannelPerlin> m_VirtualCameraDatas = new List<CinemachineBasicMultiChannelPerlin>(); 

        public override void OnPlayableDestroy(Playable playable)
        {
            if (mBrain != null)
            {
                var method =  mBrain.GetType().GetMethod("ReleaseCameraOverride", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(mBrain, new object[] {mBrainOverrideId});
                //mBrain.ReleaseCameraOverride(mBrainOverrideId); // clean up
            }
            m_IsFirstFrameHappened = false;
            mBrainOverrideId = -1;           
        }

        public override void OnGraphStop(Playable playable)
        {
            for (int i = 0; i < m_VirtualCameraDatas.Count; ++i)
            {
                var data = m_VirtualCameraDatas[i];
                if (data != null)
                {
                    data.m_AmplitudeGain = 0;
                    data.m_FrequencyGain = 0;
                    data.m_NoiseProfile = null;
                }
            }
            
            m_VirtualCameraDatas.Clear();
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            mPlaying = info.evaluationType == FrameData.EvaluationType.Playback;
        }

        struct ClipInfo
        {
            public ICinemachineCamera vcam;
            public float weight;
            public double localTime;
            public double duration;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
                      
            // Get the brain that this track controls.
            // Older versions of timeline sent the gameObject by mistake.
            GameObject go = playerData as GameObject;
            if (go == null)
                mBrain = (CinemachineBrain)playerData;
            else
                mBrain = go.GetComponent<CinemachineBrain>();
            if (mBrain == null)
                return;

            if (!m_IsFirstFrameHappened)
            {
                m_IsFirstFrameHappened = true;
                m_VirtualCameraDatas.Clear();
                m_Method = mBrain.GetType().GetMethod("SetCameraOverride", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            // Find which clips are active.  We can process a maximum of 2.
            // In the case that the weights don't add up to 1, the outgoing weight
            // will be calculated as the inverse of the incoming weight.
            int activeInputs = 0;
            ClipInfo clipA = new ClipInfo();
            ClipInfo clipB = new ClipInfo();
            for (int i = 0; i < playable.GetInputCount(); ++i)
            {
                float weight = playable.GetInputWeight(i);
                var clip = (ScriptPlayable<CameraShotBehaviourData>)playable.GetInput(i);
                CameraShotBehaviourData shot = clip.GetBehaviour();
                if (weight > 0)
                {
                    if (!shot.IsFirstFrameHappened)
                    {
                        if (shot.ChannelPerlin)
                        {
                            m_VirtualCameraDatas.Add(shot.ChannelPerlin);
                            shot.Start();
                        }

                        shot.IsFirstFrameHappened = true;
                    }
                    
                    clipA = clipB;
                    clipB.vcam = shot.VirtualCamera;
                    clipB.weight = weight;
                    clipB.localTime = clip.GetTime();
                    clipB.duration = clip.GetDuration();
                    
                    if (++activeInputs == 2)
                        break;
                }
                else
                {
                    if (shot.IsFirstFrameHappened && activeInputs != 2)
                    {                        
                        shot.Reset();
                    }  
                    shot.IsFirstFrameHappened = false;                   
                }
            }

            // Figure out which clip is incoming
            bool incomingIsB = clipB.weight >= 1 || clipB.localTime < clipB.duration / 2;
            if (activeInputs == 2)
            {
                if (clipB.localTime < clipA.localTime)
                    incomingIsB = true;
                else if (clipB.localTime > clipA.localTime)
                    incomingIsB = false;
                else 
                    incomingIsB = clipB.duration >= clipA.duration;
            }

            // Override the Cinemachine brain with our results
            ICinemachineCamera camA = incomingIsB ? clipA.vcam : clipB.vcam;
            ICinemachineCamera camB = incomingIsB ? clipB.vcam : clipA.vcam;
            float camWeightB = incomingIsB ? clipB.weight : 1 - clipB.weight;
            
            
            mBrainOverrideId = (int)m_Method.Invoke(mBrain, new object[] {mBrainOverrideId, camA, camB, camWeightB, GetDeltaTime(info.deltaTime)});
            /*mBrainOverrideId = mBrain.SetCameraOverride(
                    mBrainOverrideId, camA, camB, camWeightB, GetDeltaTime(info.deltaTime));*/
        }

        float mLastOverrideFrame;
        float GetDeltaTime(float deltaTime)
        {
            if (!mPlaying)
            {
                if (mBrainOverrideId < 0)
                    mLastOverrideFrame = -1;
                float time = Time.realtimeSinceStartup;
                deltaTime = Time.unscaledDeltaTime;
                if (!Application.isPlaying 
                    && (mLastOverrideFrame < 0 || time - mLastOverrideFrame > Time.maximumDeltaTime))
                {
                    deltaTime = -1;
                }
                mLastOverrideFrame = time;
            }
            return deltaTime;
        }
    }
}