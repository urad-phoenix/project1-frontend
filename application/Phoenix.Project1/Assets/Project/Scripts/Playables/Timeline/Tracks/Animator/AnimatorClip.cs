namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using System.Collections.Generic;
    using UnityEngine.Timeline;

    [System.Serializable]
    public class AnimatorClip : PlayableAsset
    {
        private readonly AnimatorBehaviourData template = new AnimatorBehaviourData();

        [HideInInspector] public float StartBlendingTime;
        [HideInInspector] public float EndBlendingTime;
        [HideInInspector] public float AnimationLength;
        [HideInInspector] public bool IsLoop;
        [HideInInspector] public List<string> Names;
        [HideInInspector] public Animator Animator;
        [HideInInspector] public TimelineClip TimeClip;
        [HideInInspector] public string ReturnKey;
        [HideInInspector] public bool IsReturnToSpecifyState;
        public int Layer;
        public string StateKey;
        
        public override double duration
        {
            get
            {
                if (AnimationLength == 0)
                    return base.duration;

                double length = AnimationLength;
                if (length < float.Epsilon)
                    return base.duration;
                
                return length;
            }
        }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<AnimatorBehaviourData>.Create(graph, template);
            AnimatorBehaviourData clone = playable.GetBehaviour();
            clone.StateKey = StateKey;
            clone.StartBlendingTime = StartBlendingTime;
            clone.EndBlendingTime = EndBlendingTime;
            clone.IsLoop = IsLoop;
            clone.AnimationLength = AnimationLength;
            clone.Layer = Layer;
            clone.ReturnKey = ReturnKey;
            clone.IsReturnToSpecifyState = IsReturnToSpecifyState;
            
            if (TimeClip != null)
            {
                clone.StartBlendingTime = (float) TimeClip.blendInDuration > 0 ? (float) TimeClip.blendInDuration : 0;
              
                clone.EndBlendingTime = (float) TimeClip.blendOutDuration > 0 ? (float) TimeClip.blendOutDuration : 0;                
            }

            return playable;
        }

        #if UNITY_EDITOR
        public void SetAnimator(AnimatorClip clip, Animator animator,  TimelineClip timeClip)
        {        
            var states = Editor.Utility.TimelineEditorUtility.GetStates(animator, clip.Layer);

            if (states != null && states.Length != 0)
            {
                List<string> names = new List<string>();
                for (int i = 0; i < states.Length; ++i)
                {
                    names.Add(states[i].state.name);
                }

                if (names.Count > 0)
                {
                    if (string.IsNullOrEmpty(clip.StateKey))
                    {
                        clip.StateKey = states[0].state.name;
                    }
                    clip.Names = names;
                }
            } 
                
            var anim = Editor.Utility.TimelineEditorUtility.GetAnimation(animator, clip.Layer, clip.StateKey);

            if (anim != null)
            {
                clip.IsLoop = anim.isLooping;                    
                clip.AnimationLength = anim.length;
                /*clip.StartBlendingTime = (float) timeClip.blendInDuration > 0 ? (float) timeClip.blendInDuration : 0;
                clip.EndBlendingTime = (float) timeClip.blendOutDuration > 0 ? (float) timeClip.blendOutDuration : 0;*/
            }
        }
        #endif
    }
}