namespace Phoenix.Playables
{
	using Attribute;
	using UnityEngine;

	[Binding(typeof(IAnimationBinding), BindingCategory.Animator)]
	public class AnimationTrack : UnityEngine.Timeline.AnimationTrack
	{       
        public BindingTrackType BindingType;

		public AnimationCategory Category;

		public string Key = "";
    }
}