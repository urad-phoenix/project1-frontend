using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phoenix.Playables.Markers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables
{
    public class SpineAnimationBehaviour : PlayableBehaviour
    {
        private SkeletonAnimation _Animator;
        private bool _IsFirstFrameHappened;
        private float _NormalTime;
        private string _ReturnName;
        private bool _IsReset;       
        public SpinePlayableReceiver Receiver;
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            _Animator = playerData as SkeletonAnimation;

            if(_Animator == null)
            {
                return;
            }
            
            int inputCount = playable.GetInputCount();    

            var time = playable.GetTime();                       
            
            for(int i = 0; i < inputCount; ++i)
            {
                float inputWeight = playable.GetInputWeight(i);

                var inputPlayable = (ScriptPlayable<SpineAnimationData>)playable.GetInput(i);
                
                var behaviour = inputPlayable.GetBehaviour();

                if(inputWeight > 0)
                {                                        
                    
                    if(!_IsFirstFrameHappened)
                    {           
                        _IsFirstFrameHappened = true;
                        _NormalTime = 0f;
                        _IsReset = behaviour.IsReturnToSpecifyState;
                        _ReturnName = behaviour.ReturnName;                       
                    }
                                                           
                    behaviour.SendNotification(new SpineAnimationNotification(), playable, info.output, time);

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
                            _Animator.AnimationState.SetAnimation(behaviour.Track, behaviour.Name, behaviour.IsLoop);
                        }
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

        private void ProcessByFrame(Playable playable, SpineAnimationData behaviour)
        {
            var animation = _Animator.skeletonDataAsset.GetSkeletonData(true).Animations.Find(x => x.Name == behaviour.Name);
            
            if(animation == null)
                return;
            
            if(_Animator.AnimationName != behaviour.Name)
            {
                _Animator.AnimationState.SetAnimation(behaviour.Track, behaviour.Name, true);               
            }           
            
            var preTime = (float)playable.GetPreviousTime();

            var time =  (float) playable.GetTime();            

            if(time < 0.00000f)
                return;

            var deltaTime = Mathf.Abs(time - preTime);
            
            if (!behaviour.IsLoop && time >= behaviour.Duration)
            {
                return;   
            }

            _Animator.Update(deltaTime);

            _Animator.LateUpdate();
        }

        private void ProcessByPauseFrame(Playable playable, SpineAnimationData behaviour)
        {
            _Animator.state.ClearTracks();

            if(_Animator.AnimationName != behaviour.Name)
            {
                try
                {
                    _Animator.AnimationState.SetAnimation(behaviour.Track, behaviour.Name, true);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }                
            }            

            var time = (float)playable.GetTime();

            if(time < 0.00000f)
                return;            
                
            if (!behaviour.IsLoop && time >= behaviour.Duration)
            {
                time = behaviour.Duration;
            }
            
            _Animator.Update(time);
            _Animator.LateUpdate();
        }      
        
        public override void OnGraphStart(Playable playable)
        {           
            int inputCount = playable.GetInputCount();

            for(int i = 0; i < inputCount; ++i)
            {
                var inputPlayable = (ScriptPlayable<SpineAnimationData>)playable.GetInput(i);

                var behaviour = inputPlayable.GetBehaviour();

                if (behaviour != null && behaviour.HasMarkers())
                {                    
                    behaviour.AddNotification(playable, Receiver);
                }
            }            
        }

        public override void OnGraphStop(Playable playable)
        {                          
            int inputCount = playable.GetInputCount();

            for(int i = 0; i < inputCount; ++i)
            {
                var inputPlayable = (ScriptPlayable<SpineAnimationData>)playable.GetInput(i);

                var behaviour = inputPlayable.GetBehaviour();

                if (behaviour != null)
                {
                    behaviour.IsFirstFrameHappened = false;
                    behaviour.RemoveNotification(playable);
                }
            }

            _IsFirstFrameHappened = false;

            if(!_IsReset)
                return;

            _IsReset = false;

            if(_Animator != null)
            {
                _Animator.AnimationState.SetAnimation(0, _ReturnName, true);               
            }

#if UNITY_EDITOR
            _EditorPreviewStop();
#endif
        }
#if UNITY_EDITOR
        void _EditorPreviewStop()
        {
            if(!Application.isPlaying && _Animator != null)
            {
                _Animator.Update(0);              
            }
        }
#endif
    }
}
