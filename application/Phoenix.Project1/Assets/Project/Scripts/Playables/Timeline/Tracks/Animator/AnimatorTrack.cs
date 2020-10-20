namespace Phoenix.Playables
{	
	using Attribute;	
	using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

	[Binding(typeof(IAnimationBinding), BindingCategory.Animator)]
    [TrackColor(0f, 0.4866645f, 1f)]
    [TrackClipType(typeof(AnimatorClip))]
    [TrackBindingType(typeof(Animator))]
    public class AnimatorTrack : TrackAsset, ITrackRuntimeBinding
	{
        public BindingTrackType BindingType;
		public bool IsReturnToSpecifyState;
		public string ReturnKey = "idle";		
		
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {

            var director = go.GetComponent<PlayableDirector>();
            
            Animator animator = null;
            
            if (director != null)
            {
                animator = director.GetGenericBinding(this) as Animator;
            }

	        foreach (var clip in GetClips())
	        {
		        var c = clip.asset as AnimatorClip;		        
		        c.IsReturnToSpecifyState = IsReturnToSpecifyState;
		        c.TimeClip = clip;
		        c.ReturnKey = ReturnKey;
#if UNITY_EDITOR
		        c.Animator = animator;
		        c.SetAnimator(c, animator, clip);

	        /*var states = Editor.Utility.TimelineEditorUtility.GetStates(animator, c.Layer);

	        if (states != null && states.Length != 0)
	        {
	            List<string> names = new List<string>();
	            for (int i = 0; i < states.Length; ++i)
	            {
	                names.Add(states[i].state.name);
	            }

	            if (names.Count > 0)
	            {
	                if (string.IsNullOrEmpty(c.StateKey))
	                {
	                    c.StateKey = states[0].state.name;
	                }
	                c.Names = names;
	            }
	        } 
	        
	        var anim = Editor.Utility.TimelineEditorUtility.GetAnimation(animator, c.Layer, c.StateKey);

	        if (anim != null)
	        {
	            c.IsLoop = anim.isLooping;                    
	            c.AnimationLength = anim.length;
	            c.StartBlendingTime = (float) clip.blendInDuration > 0 ? (float) clip.blendInDuration : 0;
	            c.EndBlendingTime = (float) clip.blendOutDuration > 0 ? (float) clip.blendOutDuration : 0;
	        }*/
            }

            foreach (var clip in GetClips())
            {
                var c = clip.asset as AnimatorClip;
                c.StartBlendingTime = (float) clip.blendInDuration > 0 ? (float) clip.blendInDuration : 0;           
                c.EndBlendingTime = (float) clip.blendOutDuration > 0 ? (float) clip.blendOutDuration : 0;
            }
#else
			}
#endif
            
            return ScriptPlayable<AnimatorBehaviour>.Create(graph, inputCount);
        }

		public void GetBindingData(PlayableDirector playableDirector, GameObject gameObject)
		{
		}

		public Object GetBindingKey()
		{
			return this;
		}

		public BindingCategory GetBindingType()
		{
			return BindingCategory.Animator;
		}

		public BindingTrackType GetTrackType()
		{
		    return BindingType;
		}
	}
}
