namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    
    [System.Serializable]
    public class VFXClip : PlayableAsset
    {
        private VFXBehaviourData template = new VFXBehaviourData();

        public ExposedReference<Transform> LaunchPoint;

        public ExposedReference<Transform> TargetPoint;
                       
        public ExposedReference<Transform> VFX;

        [HideInInspector]
        public Vector3 StartPosition;
        
        [HideInInspector]
        public Vector3 TargetPosition;
        
        [HideInInspector]
        public TimelineClip TimeClip;

        [System.NonSerialized]
        public string Key;

        [HideInInspector]
        public bool IsProjectile;
        
        [HideInInspector]
        public bool IsAnchor;

        public AnimationCurve Curve;
        
        public float HeightScale = 1.84f;
        
        public float AxisOffset;
        
        [HideInInspector]
        public bool IsAnchorToEndPoint;
        [HideInInspector]
        public bool IsSetEndToVfxHitDummy;
        [HideInInspector]
        public string RuntimeKey;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<VFXBehaviourData>.Create(graph, template);

            var data = playable.GetBehaviour();

            data.LaunchPoint = LaunchPoint.Resolve(graph.GetResolver());

            data.TargetPoint = TargetPoint.Resolve(graph.GetResolver());

            data.Duration = (float) (TimeClip.duration - TimeClip.easeOutDuration);

            data.VFX = VFX.Resolve(graph.GetResolver());             
            data.Key = Key;
            data.IsProjectile = IsProjectile;
            data.Curve = Curve;            
            data.HeightScale = HeightScale;
            data.AxisOffset = AxisOffset;
            data.IsAnchor = IsAnchor;
            data.IsFirstFrameHappened = false;
            data.IsAnchorToEndPoint = IsAnchorToEndPoint;
            data.IsSetEndToVfxHitDummy = IsSetEndToVfxHitDummy;
            data.RuntimeKey = RuntimeKey;        
            return playable;
        }
    }
}