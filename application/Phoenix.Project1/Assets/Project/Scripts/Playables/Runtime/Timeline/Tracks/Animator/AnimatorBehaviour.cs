namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    
    public class AnimatorBehaviour : PlayableBehaviour
    {
        private Animator m_Animator;
        private bool m_IsFirstFrameHappened;
        private float m_NormalTime;
        private string m_ReturnName;
        private bool m_IsReset;
        private bool m_IsPause;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_Animator = playerData as Animator;

            if (m_Animator == null)
            {
                return;
            }
            
            int inputCount = playable.GetInputCount();

            for (int i = 0; i < inputCount; ++i)
            {
                float inputWeight = playable.GetInputWeight(i);

                var inputPlayable = (ScriptPlayable<AnimatorBehaviourData>) playable.GetInput(i);

                var behaviour = inputPlayable.GetBehaviour();

                if (inputWeight > 0)
                {
                   /* if (!behaviour.IsFirstFrameHappened)
                    {
                        behaviour.IsFirstFrameHappened = true;
                    }*/
                    
                    //ProcessByFrame(inputPlayable, behaviour);

                    if (!m_IsFirstFrameHappened)
                    {
                        m_IsPause = false;
                        m_IsFirstFrameHappened = true;
                        m_NormalTime = 0f;
                        m_IsReset = behaviour.IsReturnToSpecifyState;
                        m_ReturnName = behaviour.ReturnKey;
                    }
                    
                    if (!Application.isPlaying)
                    {
                        ProcessByFrame(inputPlayable, behaviour);
                    }
                    else
                    {
                        if (!behaviour.IsFirstFrameHappened)
                        {                            
                            behaviour.IsFirstFrameHappened = true;                                             
                            m_Animator.CrossFade(behaviour.StateKey,behaviour.StartBlendingTime, behaviour.Layer);                                                        
                        }

                        if (m_IsPause)
                        {
                            m_IsPause = false;
                            m_Animator.Play(behaviour.StateKey);
                        }
                    }
                }
                else
                {
                    if (behaviour.IsFirstFrameHappened)
                    {               
                        behaviour.IsFirstFrameHappened = false;                            
                    }
                }
            }
        }
        
        protected void ProcessByFrame(Playable playable, AnimatorBehaviourData behaviour)
        {         
            float progress = (float) (playable.GetTime() / playable.GetDuration());                                                             
            
            float t = 0.0f;
            if (progress > behaviour.AnimationLength)
            {
                if (behaviour.IsLoop)
                    t = (behaviour.AnimationLength % progress) - m_NormalTime;
                else
                    t = behaviour.AnimationLength;
            }
            else
            {
                t = progress;
            }
            
            m_Animator.CrossFade(behaviour.StateKey, behaviour.StartBlendingTime, behaviour.Layer, t);
            m_Animator.Update(t); 
        }
        
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {            
            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; ++i)
            {
                var inputPlayable = (ScriptPlayable<AnimatorBehaviourData>) playable.GetInput(i);
                
                var behaviour = inputPlayable.GetBehaviour();
                
                behaviour.IsFirstFrameHappened = false;
            }

            m_IsPause = true;
        }   

        public override void OnPlayableDestroy(Playable playable)
        {
            m_IsFirstFrameHappened = false;

            m_IsPause = false;
                                   
            if(!m_IsReset)
                return;
            
            m_IsReset = false;
            
            if(m_Animator != null)
                m_Animator.Play(m_ReturnName);
        }
    }
}

