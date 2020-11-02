using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Animation = Spine.Animation;

namespace Phoenix.Playables
{
    [Serializable]
    public class SpineAnimationClip : PlayableAsset, ITimelineClipAsset
    {                
        public SpineAnimationData template = new SpineAnimationData();

//        [HideInInspector] public float StartBlendingTime;
//        [HideInInspector] public float EndBlendingTime;
//        [HideInInspector] public float Duration;
        [HideInInspector] public List<string> Names;
//        [HideInInspector] public SkeletonAnimation Animator;
        //[HideInInspector] public TimelineClip TimeClip;

        [HideInInspector] public SkeletonAnimation Animator;
//        [HideInInspector] public string ReturnName;
//        [HideInInspector] public bool IsReturnToSpecifyState;
//        public int Track;
//        public string Name;
//        public bool IsLoop;

        public ClipCaps clipCaps
        {
            get
            {
                if(template.IsLoop)
                {
                    return ClipCaps.Blending | ClipCaps.ClipIn | ClipCaps.Extrapolation | ClipCaps.SpeedMultiplier | ClipCaps.Looping;
                }
                else
                {
                    return ClipCaps.Blending | ClipCaps.ClipIn | ClipCaps.Extrapolation | ClipCaps.SpeedMultiplier;
                }
            }
        }
                      
        public override double duration
        {
            get
            {               
                double length = template.Duration;
                
                if(length < float.Epsilon)
                    return base.duration;

                return length;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<SpineAnimationData>.Create(graph, template);

            return playable;
        }
        
#if UNITY_EDITOR
        public void SetAnimator(SpineAnimationClip clip, SkeletonAnimation animator)
        {
            //var states = Editor.Utility.TimelineEditorUtility.GetStates(animator, clip.Layer);

            if(animator == null)
                return;

            Animator = animator;
            
            Names = new List<string>();

            var data = animator.SkeletonDataAsset.GetSkeletonData(true);

            foreach(Animation animation in data.Animations)
            {
                Names.Add(animation.Name);
            }

            if(Names.Count > 0)
            {
                if(string.IsNullOrEmpty(clip.template.Name))
                {
                    clip.template.Name = Names[0];				  
                }			 
            }          

            var anim = data.FindAnimation(clip.template.Name);
            
            if(anim != null)
            {                
                clip.template.Duration = anim.Duration;
            }
        }
#endif        
    }
}
