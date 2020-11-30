namespace Phoenix.Playables
{
	using System;
    using UnityEngine;
	using UnityEngine.Assertions;
	using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class TransformTweenClip : PlayableAsset, ITimelineClipAsset
    {
	    public bool IsInvert;
        [Tooltip("整個Timeline表演完後Reset")]
	    public bool IsFullEndReset = false;
        [Tooltip("Clip表演完後Reset")]
        public bool IsClipEndReset = false;
        public TransformTweenBehaviourData template = new TransformTweenBehaviourData();
        public ExposedReference<Transform> startLocation;
        public ExposedReference<Transform> endLocation;
        public float Step_away_Targeter = 1.5f;

        //public Vector3 endeulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 endScale = new Vector3(1.0f, 1.0f, 1.0f);

        [Serializable]
        public class TimelineTransform
        {
			public TimelineTransform() { Position = Vector3.zero; }
			public TimelineTransform(Transform transform)
			{
				Assert.IsNotNull(transform);
				Position = new Vector3(transform.position.x, transform.position.y, transform.position.z) ;
				Scale = transform.localScale;
				Rotation = transform.rotation;
			}

			public Vector3 Position;
            public Vector3 Scale;
            public Quaternion Rotation;
        }

        [HideInInspector]
        public TimelineTransform StartTransform;
        [HideInInspector]
        public TimelineTransform TargetTransform;


        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TransformTweenBehaviourData>.Create(graph);
            TransformTweenBehaviourData clone = playable.GetBehaviour();

            var startTransform = startLocation.Resolve(graph.GetResolver());

            var endTransform = endLocation.Resolve(graph.GetResolver());

	        clone.tweenPosition = template.tweenPosition;
	        clone.IsLockAt = template.IsLockAt;
	        clone.tweenScale = template.tweenScale;
	        clone.IsInvert = IsInvert;
	        clone.IsFullEndReset = IsFullEndReset;
            clone.IsClipEndReset = IsClipEndReset;
            clone.customCurve = template.customCurve;
	        //clone.endeulerAngles = template.endeulerAngles;
	        clone.endScale = template.endScale;
	        clone.Step_away_Targeter = template.Step_away_Targeter;
	        
            if(startTransform != null)
                SetTransform(clone.StartTransform, startTransform);
            else
            {
	            clone.StartTransform = StartTransform;	            
            }
	        
			if(endTransform != null)
				SetTransform(clone.TargetTransform, endTransform);
			else
			{
				clone.TargetTransform = TargetTransform;
			}

            //clone.endeulerAngles = endeulerAngles;
            clone.endScale = endScale;
            clone.Step_away_Targeter = Step_away_Targeter;
	        clone.IsFirstFrameHappened = false;
            return playable;
        }

        private void SetTransform(TimelineTransform data, Transform transform)
        {	        
            data.Position = transform.position;
            data.Rotation = transform.rotation;
            data.Scale = transform.localScale;
        }
    }
}