namespace Phoenix.Playables
{	
	using UnityEngine.Timeline;
	using UnityEngine;

	[TrackClipType(typeof (ControlClip))]
    [TrackColor(0.2f, 0.4866645f, 0.2f)]
    public class ControlTrack : TrackAsset, ITrackRuntimeBinding
    {
		public Object GetBindingKey()
		{
			return this;
		}

		public BindingCategory GetBindingType()
        {
            return BindingCategory.GameObject;
        }

        public BindingTrackType GetTrackType()
        {
            return BindingTrackType.None;
        }
    }
}
