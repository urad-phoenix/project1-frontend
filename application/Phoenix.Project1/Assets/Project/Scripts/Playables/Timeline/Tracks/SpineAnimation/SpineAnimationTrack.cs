using Phoenix.Playables.Attribute;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables
{
    [Binding(typeof(IAnimationBinding), BindingCategory.Animator)]
    [TrackColor(0f, 0, 0.5187078f)]
    [TrackClipType(typeof(SpineAnimationClip))]
    [TrackBindingType(typeof(SkeletonAnimation))]

    public class SpineAnimationTrack : TrackAsset, ITrackRuntimeBinding
    {
		public BindingTrackType BindingType;
		public bool IsReturnToSpecifyState;
		[Tooltip("播放完回到某個動畫")]
		public string ReturnName = "idle";
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{

			var director = go.GetComponent<PlayableDirector>();

			SkeletonAnimation animator = null;

			if(director != null)
			{
				animator = director.GetGenericBinding(this) as SkeletonAnimation;
			}
			foreach(var clip in GetClips())
			{
				var c = clip.asset as SpineAnimationClip;
				c.IsReturnToSpecifyState = IsReturnToSpecifyState;
				c.TimeClip = clip;
				c.TimeClip.displayName = c.name;
				c.ReturnName = ReturnName;
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

			foreach(var clip in GetClips())
			{
				var c = clip.asset as SpineAnimationClip;
				c.StartBlendingTime = (float)clip.blendInDuration > 0 ? (float)clip.blendInDuration : 0;
				c.EndBlendingTime = (float)clip.blendOutDuration > 0 ? (float)clip.blendOutDuration : 0;
			}
#else
			}
#endif			
			//animator.

			return ScriptPlayable<SpineAnimationBehaviour>.Create(graph, inputCount);
		}		

		public Object GetBindingKey()
		{
			return this;
		}

		public BindingCategory GetBindingType()
		{
			return BindingCategory.SpineAnimation;
		}

		public BindingTrackType GetTrackType()
		{
			return BindingType;
		}
	}
}