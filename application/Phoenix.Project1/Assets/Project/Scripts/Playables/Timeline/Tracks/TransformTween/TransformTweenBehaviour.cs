﻿namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;

    public class TransformTweenBehaviour : PlayableBehaviour
    {
        bool m_FirstFrameHappened;
        private Vector3 m_DefaultPosition;
        private Quaternion m_DefaultRotation;
        private Vector3 m_DefaultScale;
        private Transform m_TrackBinding;
        private bool m_IsReset;
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            m_TrackBinding = playerData as Transform;

            if (m_TrackBinding == null)
                return;

            int inputCount = playable.GetInputCount();

            //float positionTotalWeight = 0f;
            //float rotationTotalWeight = 0f;
            //float scaleTotalWeight = 1f;

            if (!m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;
                m_DefaultPosition = m_TrackBinding.position;
                m_DefaultScale = m_TrackBinding.localScale;
                m_DefaultRotation = m_TrackBinding.rotation;
                m_IsReset = false;
            }            
            
            Vector3 blendedPosition = m_DefaultPosition;
            Vector3 blendedScale = m_DefaultScale;
            Quaternion blendedRotation = m_DefaultRotation;
            
            for (int i = 0; i < inputCount; i++)
            {
           
                ScriptPlayable<TransformTweenBehaviourData> playableInput = (ScriptPlayable<TransformTweenBehaviourData>)playable.GetInput(i);
                TransformTweenBehaviourData input = playableInput.GetBehaviour();
                
                if(input.TargetTransform == null || input.StartTransform == null)
                    continue;                                
              
                float inputWeight = playable.GetInputWeight(i);

                Vector3 startPoint = input.IsInvert ? input.TargetTransform.Position - (( input.TargetTransform.Position -  input.StartTransform.Position).normalized) * input.Step_away_Targeter : 
                    input.StartTransform.Position;

                Quaternion startRotation = input.IsInvert ? input.TargetTransform.Rotation : input.StartTransform.Rotation;
                Vector3 startScale = input.IsInvert ? input.endScale : input.StartTransform.Scale;
                Vector3 endPoint = input.IsInvert ? input.StartTransform.Position : input.TargetTransform.Position - ((input.TargetTransform.Position - startPoint).normalized) * input.Step_away_Targeter;;
                Quaternion endRotation = input.IsInvert ? input.StartTransform.Rotation : input.TargetTransform.Rotation;
                Vector3 endScale = input.IsInvert ? input.StartTransform.Scale : input.endScale;
                                    
                //Quaternion endQuaternion = Quaternion.Euler(startRotation.eulerAngles + input.endeulerAngles);
                
                /*Vector3 blendedPosition = startPoint;
                Vector3 blendedScale = startScale;
                Quaternion blendedRotation = startRotation;*/
                
                if (inputWeight < 1)
                {
                    if (input.IsFirstFrameHappened)
                    {
                        input.IsFirstFrameHappened = false;
                        if(!input.IsClipEndReset)
                        {
                            if(input.tweenPosition)
                                m_TrackBinding.position = endPoint;

                            if(input.IsLockAt)
                                m_TrackBinding.rotation = endRotation;

                            if(input.tweenScale)
                                m_TrackBinding.localScale = endScale;
                        }
                        else
                        {
                            if(input.tweenPosition)
                                m_TrackBinding.position = startPoint;

                            if(input.IsLockAt)
                                m_TrackBinding.rotation = startRotation;

                            if(input.tweenScale)
                                m_TrackBinding.localScale = startScale;
                        }

                    }                    
                    continue;
                }

                if (!input.IsFirstFrameHappened)
                {
                    input.IsFirstFrameHappened = true;
                    m_TrackBinding.position = startPoint;                                     
                    m_TrackBinding.localScale = startScale;
                    
                    if (input.IsLockAt)
                    {
                        var dir = endPoint -  m_TrackBinding.position;                       
                        //Quaternion endQuaternion = Quaternion.Euler(startRotation.eulerAngles + input.endeulerAngles);
                        var rotation = Quaternion.LookRotation(dir, Vector3.up);
                        m_TrackBinding.rotation = rotation;
                    }

                    if (input.IsFullEndReset)
                        m_IsReset = true;
                }

                double normalisedTime = (playableInput.GetTime() / playableInput.GetDuration());
                //Debug.Log(normalisedTime);
                float tweenProgress = input.EvaluateCurrentCurve((float)normalisedTime);
                if (tweenProgress > 0.96f) { tweenProgress = 1.0f;}
                if (input.tweenPosition)
                {
                    //positionTotalWeight += inputWeight;

                    blendedPosition = Vector3.Lerp(startPoint, endPoint, (float) tweenProgress);// * inputWeight;
                    
                }
                if (input.tweenScale)
                {
                    //scaleTotalWeight = inputWeight;
                    blendedScale = (Vector3.Lerp(startScale, endScale, (float)tweenProgress));

                }
                /*if (input.tweenRotation)
                {
                    rotationTotalWeight += inputWeight;

                    Quaternion desiredRotation = Quaternion.Lerp(startRotation, endQuaternion, (float)tweenProgress);
                    desiredRotation = NormalizeQuaternion(desiredRotation);

                    if (Quaternion.Dot(blendedRotation, desiredRotation) < 0f)
                    {
                        desiredRotation = ScaleQuaternion(desiredRotation, -1f);
                    }

                    desiredRotation = ScaleQuaternion(desiredRotation, inputWeight);

                    blendedRotation = AddQuaternions(blendedRotation, desiredRotation);
                }*/
                
                //blendedPosition += m_DefaultPosition * (1f - positionTotalWeight);
                //blendedScale = defaultScale * (scaleTotalWeight);
                //Quaternion weightedDefaultRotation = m_DefaultRotation;// ScaleQuaternion(m_DefaultRotation, 1f - rotationTotalWeight);
                //blendedRotation = AddQuaternions(blendedRotation, weightedDefaultRotation);
                m_TrackBinding.position = blendedPosition;
            
                //trackBinding.position = new Vector3(0,0,5.7f) ;
                //m_TrackBinding.rotation = blendedRotation;
                m_TrackBinding.localScale = blendedScale;
            }  

           // m_FirstFrameHappened = true;
        }

        public override void OnGraphStop(Playable playable)
        {
            Debug.Log("OnGraphStop");
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Debug.Log("OnBehaviourPause");
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            Debug.Log("OnGraphStop");
        }        

        public override void OnPlayableDestroy(Playable playable)
        {          
            m_FirstFrameHappened = false;
            if (m_IsReset)
            {
                if (m_TrackBinding != null)
                {
                    m_TrackBinding.position = m_DefaultPosition;
                    m_TrackBinding.localScale = m_DefaultScale;
                    m_TrackBinding.rotation = m_DefaultRotation;
                }
            }

            m_IsReset = false;
        }

        static Quaternion AddQuaternions(Quaternion first, Quaternion second)
        {
            first.w += second.w;
            first.x += second.x;
            first.y += second.y;
            first.z += second.z;
            return first;
        }

        static Quaternion ScaleQuaternion(Quaternion rotation, float multiplier)
        {
            rotation.w *= multiplier;
            rotation.x *= multiplier;
            rotation.y *= multiplier;
            rotation.z *= multiplier;
            return rotation;
        }

        static float QuaternionMagnitude(Quaternion rotation)
        {
            return Mathf.Sqrt((Quaternion.Dot(rotation, rotation)));
        }

        static Quaternion NormalizeQuaternion(Quaternion rotation)
        {
            float magnitude = QuaternionMagnitude(rotation);

            if (magnitude > 0f)
                return ScaleQuaternion(rotation, 1f / magnitude);

            Debug.LogWarning("Cannot normalize a quaternion with zero magnitude.");
            return Quaternion.identity;
        }
    }
}