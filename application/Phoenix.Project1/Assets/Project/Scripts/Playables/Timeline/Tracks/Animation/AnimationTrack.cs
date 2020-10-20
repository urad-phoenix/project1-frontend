namespace Phoenix.Playables
{
	using Attribute;
	using UnityEngine;

	[Binding(typeof(IAnimationBinding), BindingCategory.Animator)]
	public class AnimationTrack : UnityEngine.Timeline.AnimationTrack, ITrackRuntimeBinding
	{       
        public BindingTrackType BindingType;

		public AnimationCategory Category;

		public string Key = "";

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