namespace Phoenix.Playables
{    
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Attribute;
       
    [Binding(typeof(IVFXBinding), BindingCategory.VFX)]
    [TrackClipType(typeof (VFXClip))]
    [TrackColor(0.2f, 0.4866645f, 0.2f)]
    public class VFXTrack : TrackAsset, ITrackRuntimeBinding
    {
        public BindingTrackType StartBindingType;        
        public BindingTrackType EndBindingType;
        public string VFXKey;
        public bool IsProjectile;
        public bool IsAnchor;
        public string AnchorDummyKey;
        public bool IsAnchorToEndPoint;
        /*當IsAnchorToEndPoint = true, 會判斷
        if true endPoint會吃vfxhitDummy , if false 則吃角色本身位置*/
        public bool IsSetEndToVfxHitDummy = true;
        public int StartDummyKey = 4;
        public int TargetDummyKey = 5;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {                       
            foreach (var clip in GetClips())
            {
                var c = clip.asset as VFXClip;
                c.TimeClip = clip;

                c.IsProjectile = IsProjectile;
                c.IsAnchor = IsAnchor;
                c.Key = VFXKey;
                c.IsAnchorToEndPoint = IsAnchorToEndPoint;
                c.IsSetEndToVfxHitDummy = IsSetEndToVfxHitDummy;
            }

            return ScriptPlayable<VFXBehaviour>.Create(graph, inputCount);
        }

		public Object GetBindingKey()
		{
			return this;
		}

		public BindingCategory GetBindingType()
        {
            return BindingCategory.VFX;
        }

        public BindingTrackType GetTrackType()
        {
            return BindingTrackType.None;
        }
    }
}
