using System.Linq;
using Phoenix.Playables.Attribute;
using Phoenix.Playables.Markers;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;


namespace Phoenix.Playables
{
    [Binding(typeof(IAnimationBinding), BindingCategory.Animator)]
    [TrackColor(0f, 0, 0.5187078f)]
    [TrackClipType(typeof(SpineAnimationClip))]
    [TrackBindingType(typeof(SkeletonAnimation))]	
    public class SpineAnimationTrack : BaseTrack
    {
		public BindingTrackType BindingType;
		public bool IsReturnToSpecifyState = true;
		[Tooltip("播放完回到某個動畫")]
		public string ReturnName = "idle";
	    
#pragma warning disable 0649
	    [SerializeField] private SpineAnimationBehaviour template;
#pragma warning restore 0649
	    
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			var director = go.GetComponent<PlayableDirector>();

			SkeletonAnimation animator = null;

			var playable = ScriptPlayable<SpineAnimationBehaviour>.Create(graph, template, inputCount);

			var behaviour = playable.GetBehaviour();
			
			behaviour.Receiver = new SpinePlayableReceiver();					
			
			SetMarker(behaviour);
			
			if(director != null)
			{
				animator = director.GetGenericBinding(this) as SkeletonAnimation;								
			}		
			
			foreach(var clip in GetClips())
			{
				var c = clip.asset as SpineAnimationClip;
								
				c.template.IsReturnToSpecifyState = IsReturnToSpecifyState;					
				
				c.template.ReturnName = ReturnName;
#if UNITY_EDITOR
				
				//c.template.Animator = animator;

				c.SetAnimator(c, animator);
				
				clip.displayName = c.template.Name;   							
			}					
#else				
			}
#endif

			return playable;
		}	    		
	}
}