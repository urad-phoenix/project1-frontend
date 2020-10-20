using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables
{
    public class SpineAnimationBehaviour : PlayableBehaviour
    {
        private SkeletonAnimation m_Animator;
        private bool m_IsFirstFrameHappened;
        private float m_NormalTime;
        private string m_ReturnName;
        private bool m_IsReset;
        private bool m_IsPause;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_Animator = playerData as SkeletonAnimation;

            if(m_Animator == null)
            {
                return;
            }

            int inputCount = playable.GetInputCount();            

            for(int i = 0; i < inputCount; ++i)
            {
                float inputWeight = playable.GetInputWeight(i);

                var inputPlayable = (ScriptPlayable<SpineAnimationData>)playable.GetInput(i);
                
                var behaviour = inputPlayable.GetBehaviour();

                if(inputWeight > 0)
                {
                    if(!m_IsFirstFrameHappened)
                    {
                        m_IsPause = false;
                        m_IsFirstFrameHappened = true;
                        m_NormalTime = 0f;
                        m_IsReset = behaviour.IsReturnToSpecifyState;
                        m_ReturnName = behaviour.ReturnName;
                    }

                    if(!Application.isPlaying)
                    {
                        if(info.deltaTime <= 0.00000f)
                        {
                            ProcessByPauseFrame(inputPlayable, behaviour);
                        }
                        else
                        {
                            ProcessByFrame(inputPlayable, behaviour);
                        }
                    }
                    else
                    {
                        if(!behaviour.IsFirstFrameHappened)
                        {
                            behaviour.IsFirstFrameHappened = true;
                            //m_Animator.CrossFade(behaviour.StateKey, behaviour.StartBlendingTime, behaviour.Layer);
                            m_Animator.AnimationState.SetAnimation(behaviour.Track, behaviour.Name, behaviour.IsLoop);
                        }

                        //if(m_IsPause)
                        //{
                        //    m_IsPause = false;
                        //    m_Animator.Play(behaviour.StateKey);
                        //}
                    }
                }
                else
                {
                    if(behaviour.IsFirstFrameHappened)
                    {
                        behaviour.IsFirstFrameHappened = false;
                    }
                }
            }
        }

        public override void OnGraphStart(Playable playable)
        {
            Debug.Log("OnGraphStart");
        }

        protected void ProcessByFrame(Playable playable, SpineAnimationData behaviour)
        {               
            if(m_Animator.AnimationName != behaviour.Name)
            {
                m_Animator.AnimationState.SetAnimation(behaviour.Track, behaviour.Name, true);               
            }           

            var preTime = (float)playable.GetPreviousTime();

            var time =  (float) playable.GetTime();            

            if(time < 0.00000f)
                return;

            var deltaTime = Mathf.Abs(time - preTime);

            m_Animator.Update(deltaTime);

            m_Animator.LateUpdate();
        }

        protected void ProcessByPauseFrame(Playable playable, SpineAnimationData behaviour)
        {
            m_Animator.state.ClearTracks();

            if(m_Animator.AnimationName != behaviour.Name)
            {
                m_Animator.AnimationState.SetAnimation(behaviour.Track, behaviour.Name, true);
            }            

            var time = (float)playable.GetTime();

            if(time < 0.00000f)
                return;

            //var deltaTime = Mathf.Abs(time - preTime);
            //m_Animator.skeleton.Time = 0;            
          
            m_Animator.Update(time);
            m_Animator.LateUpdate();
        }

        public override void OnPlayableCreate(Playable playable)
        {
            Debug.Log("OnPlayableCreate");
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log("OnPlayableCreate");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log("OnBehaviourPause");
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
            {
                m_Animator.AnimationState.SetAnimation(0, m_ReturnName, true);                
            }

            int inputCount = playable.GetInputCount();

            for(int i = 0; i < inputCount; ++i)
            {
                var inputPlayable = (ScriptPlayable<SpineAnimationData>)playable.GetInput(i);               

                var behaviour = inputPlayable.GetBehaviour();

                behaviour.IsFirstFrameHappened = false;                             
            }

#if UNITY_EDITOR
            _EditorPreviewStop();
#endif
        }

        public override void OnGraphStop(Playable playable)
        {

            Debug.Log("OnGraphStop");
            int inputCount = playable.GetInputCount();

            for(int i = 0; i < inputCount; ++i)
            {
                var inputPlayable = (ScriptPlayable<SpineAnimationData>)playable.GetInput(i);

                var behaviour = inputPlayable.GetBehaviour();

                if(behaviour != null)
                    behaviour.IsFirstFrameHappened = false;
            }

            m_IsFirstFrameHappened = false;

            if(!m_IsReset)
                return;

            m_IsReset = false;

            if(m_Animator != null)
            {
                m_Animator.AnimationState.SetAnimation(0, m_ReturnName, true);               
            }

#if UNITY_EDITOR
            _EditorPreviewStop();
#endif
        }
#if UNITY_EDITOR
        void _EditorPreviewStop()
        {
            if(!Application.isPlaying && m_Animator != null)
            {
                //m_Animator.skeleton.SetBonesToSetupPose();
                //m_Animator.skeleton.Time = 0;
                m_Animator.Update(0);              
            }
        }
#endif
    }
}
