namespace Phoenix.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Playables;

    [Serializable]
    public class TransformTweenBehaviourData : PlayableBehaviour
    {
        public enum TweenType
        {
            Linear,
            Deceleration,
            Harmonic,
            Custom,
        }

        //public Transform startLocation;
        //public Transform endLocation;
        public bool IsInvert;
        public float Step_away_Targeter ;
        //public Vector3 endeulerAngles;
        public Vector3 endScale;
        public bool tweenPosition = true;
        public bool IsLockAt = true;
        public bool tweenScale = false;
        public TweenType tweenType;
        public AnimationCurve customCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public bool IsFirstFrameHappened;
        public bool IsFullEndReset;
        public bool IsClipEndReset;

        /*public Vector3 startingPosition;
        public Vector3 startingScale = Vector3.one;
        public Quaternion startingRotation = Quaternion.identity;*/
        
        public TransformTweenClip.TimelineTransform StartTransform = new TransformTweenClip.TimelineTransform();
        public TransformTweenClip.TimelineTransform TargetTransform = new TransformTweenClip.TimelineTransform(); 

        AnimationCurve m_LinearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        AnimationCurve m_DecelerationCurve = new AnimationCurve
        (
            new Keyframe(0f, 0f, -k_RightAngleInRads, k_RightAngleInRads),
            new Keyframe(1f, 1f, 0f, 0f)
        );
        AnimationCurve m_HarmonicCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        const float k_RightAngleInRads = Mathf.PI * 0.5f;               

        /*public override void PrepareFrame(Playable playable, FrameData info)
        {
            if (StartTransform != null)
            {
                startingPosition = StartTransform.Position;
                startingRotation = StartTransform.Rotation;
                startingScale = StartTransform.Scale;
            }
        }*/

        public float EvaluateCurrentCurve(float time)
        {
            if (tweenType == TweenType.Custom && !IsCustomCurveNormalised())
            {
                Debug.LogError("Custom Curve is not normalised.  Curve must start at 0,0 and end at 1,1.");
                return 0f;
            }

            switch (tweenType)
            {
                case TweenType.Linear:
                    return m_LinearCurve.Evaluate(time);
                case TweenType.Deceleration:
                    return m_DecelerationCurve.Evaluate(time);
                case TweenType.Harmonic:
                    return m_HarmonicCurve.Evaluate(time);
                default:
                    return customCurve.Evaluate(time);
            }
        }

        bool IsCustomCurveNormalised()
        {
            if (!Mathf.Approximately(customCurve[0].time, 0f))
                return false;

            if (!Mathf.Approximately(customCurve[0].value, 0f))
                return false;

            if (!Mathf.Approximately(customCurve[customCurve.length - 1].time, 1f))
                return false;

            return Mathf.Approximately(customCurve[customCurve.length - 1].value, 1f);
        }
    }
}