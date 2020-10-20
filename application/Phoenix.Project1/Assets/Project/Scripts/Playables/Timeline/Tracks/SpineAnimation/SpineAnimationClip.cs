using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Animation = Spine.Animation;

namespace Phoenix.Playables
{
    public class SpineAnimationClip : PlayableAsset
    {
        private readonly SpineAnimationData template = new SpineAnimationData();

        [HideInInspector] public float StartBlendingTime;
        [HideInInspector] public float EndBlendingTime;
        [HideInInspector] public float Duration;
        [HideInInspector] public List<string> Names;
        [HideInInspector] public SkeletonAnimation Animator;
        [HideInInspector] public TimelineClip TimeClip;
        [HideInInspector] public string ReturnName;
        [HideInInspector] public bool IsReturnToSpecifyState;
        public int Track;
        public string Name;
        public bool IsLoop;

        public ClipCaps clipCaps
        {
            get
            {
                if(Animator != null && Animator.skeletonDataAsset != null && IsLoop)
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
                if(Duration == 0)
                    return base.duration;

                double length = Duration;
                if(length < float.Epsilon)
                    return base.duration;

                return length;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<SpineAnimationData>.Create(graph, template);
            SpineAnimationData clone = playable.GetBehaviour();
            clone.Name = Name;
            clone.StartBlendingTime = StartBlendingTime;
            clone.EndBlendingTime = EndBlendingTime;
            clone.IsLoop = IsLoop;
            clone.Duration = Duration;
            clone.Track = Track;
            clone.ReturnName = ReturnName;
            clone.IsReturnToSpecifyState = IsReturnToSpecifyState;

            if(TimeClip != null)
            {
                clone.StartBlendingTime = (float)TimeClip.blendInDuration > 0 ? (float)TimeClip.blendInDuration : 0;

                clone.EndBlendingTime = (float)TimeClip.blendOutDuration > 0 ? (float)TimeClip.blendOutDuration : 0;
            }

            return playable;
        }

#if UNITY_EDITOR
        public void SetAnimator(SpineAnimationClip clip, SkeletonAnimation animator, TimelineClip timeClip)
        {
            //var states = Editor.Utility.TimelineEditorUtility.GetStates(animator, clip.Layer);
            List<string> names = new List<string>();

            var data = animator.SkeletonDataAsset.GetSkeletonData(true);

            foreach(Animation animation in data.Animations)
            {
                names.Add(animation.Name);
            }

            if(names.Count > 0)
            {
                if(string.IsNullOrEmpty(clip.Name))
                {
                    clip.Name = names[0];
                }
                clip.Names = names;
            }


            var anim = data.FindAnimation(clip.Name);

            if(anim != null)
            {                
                clip.Duration = anim.Duration;                
            }
        }
#endif
    }
}
