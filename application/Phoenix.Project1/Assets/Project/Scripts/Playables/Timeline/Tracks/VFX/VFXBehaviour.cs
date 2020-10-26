using System;
using System.Collections.Generic;
using Phoenix.Playables.Markers;

namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    
    [Serializable]
    public class VFXBehaviour : BaseBehaviour
    {
        private bool _FirstFrame;
        private bool _IsFirstFrameHappened;
        public VFXPlayableReceiver Receiver;            
        private string _Key;
        private List<Transform> _VFXList = new List<Transform>();
        private Transform _DefaultParent;
        
#if UNITY_EDITOR
        public class ParticleData
        {
            public ParticleSystem ParticleSystem;            
            private float _LastTime = -1f;
            private uint _RandomSeed = 1;
            private float _SystemTime;
            
            public void Simulate(float time)
            {
                if ((UnityEngine.Object) ParticleSystem != (UnityEngine.Object) null)
                {
                    // float time = (float) playable.GetTime<Playable>();
                    if (!Mathf.Approximately(_LastTime, -1f) && Mathf.Approximately(_LastTime, time))
                        return;
                    float num1 = Time.fixedDeltaTime * 0.5f;
                    float t1 = time;
                    float num2 = t1 - this._LastTime;
                    float num3 =ParticleSystem.main.startDelay.Evaluate((float) ParticleSystem.randomSeed);
                    float num4 = ParticleSystem.main.duration + num3;
                    float num5 = (double) t1 <= (double) num4 ? this._SystemTime - num3 : this._SystemTime;
                    if ((double) t1 < (double) this._LastTime || (double) t1 < (double) num1 ||
                        Mathf.Approximately(this._LastTime, -1f) ||
                        (double) num2 > (double) ParticleSystem.main.duration ||
                        (double) Mathf.Abs(num5 - ParticleSystem.time) >= (double) Time.maximumParticleDeltaTime)
                    {
                        ParticleSystem.Simulate(0.0f, true, true);
                        ParticleSystem.Simulate(t1, true, false);
                        this._SystemTime = t1;
                    }
                    else
                    {
                        float num6 = (double) t1 <= (double) num4 ? num4 : ParticleSystem.main.duration;
                        float num7 = t1 % num6;
                        float t2 = num7 - this._SystemTime;
                        if ((double) t2 < -(double) num1)
                            t2 = num7 + num4 - this._SystemTime;
                        ParticleSystem.Simulate(t2, true, false);
                        this._SystemTime += t2;
                    }

                    this._LastTime = time;    
                } 
            }
            
            public void SetRandomSeed()
            {
                if (ParticleSystem == null)
                    return;
                ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            
            public void Initialize()
            {
                _SystemTime = 0.0f;
                SetRandomSeed();
            }
        }                       
        
        private void EditorMod(float time, VFXBehaviourData data)
        {
            if (!Application.isPlaying)
            {
                if (data != null)
                    data.ParticleData.Simulate(time);
            }
        }      
#endif

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {          
            var count =  playable.GetInputCount();

            if (!_IsFirstFrameHappened)
            {
                _IsFirstFrameHappened = true;
                _Key = "";
            
                _DefaultParent = null;
            
                _VFXList.Clear();
            }                                     
            
            for (int i = 0; i < count; ++i)
            {
                var weight = playable.GetInputWeight(i);

                var input = (ScriptPlayable<VFXBehaviourData>)playable.GetInput(i);

                var data = input.GetBehaviour();
                
                SendNotification(new VFXNotification(), playable, info.output, playable.GetTime());
                
                if (weight > 0)
                {
                    float time = (float) input.GetTime();
                                  
                    if(data.VFX == null)
                        continue;
                    
                    if (data.IsFirstFrameHappened == false)
                    {
                        if(string.IsNullOrEmpty(_Key))
                            _Key = data.RuntimeKey;
                        
                        if(_DefaultParent == null)
                            _DefaultParent = data.DefaultParent;
                                                
                        data.IsFirstFrameHappened = true;
                        data.Init();
                        _VFXList.Add(data.VFX);
                    }

#if UNITY_EDITOR
                    EditorMod(time, data);
#endif                   

                    if (data.Projectile == null)
                    {
                        if (data.IsProjectile)
                        {                           
                            var position = data.VFX.position;
                        
                            var progress = (time / data.Duration);

                            if (progress < 1f)
                            {
                                var newPosition = data.DefaultPosition;

                                newPosition += (progress) * data.DistanceToTarget;
                        
                                data.VFX.position = newPosition;
                        
                                data.VFX.rotation = data.DefaultRotation;
                                newPosition.y = (data.Curve.Evaluate(progress)) * data.HeightScale;
                                data.VFX.Translate(Vector3.up * (data.Curve.Evaluate(progress)) * data.HeightScale, Space.Self);
                                data.VFX.forward = (data.VFX.position - position).normalized;    
                            }
                        }
                    }
                    else
                    {
                        data.Projectile.Shoot(time);   
                    }                    
                }
                else
                {
                    if (data.IsFirstFrameHappened)
                    {  
                        data.IsFirstFrameHappened = false;
                        _VFXList.Remove(data.VFX);
                        data.Finished();
                    }
                }
            }
        }

        public override void OnGraphStart(Playable playable)
        {           
            if (HasMarkers())
            {
                AddNotification(playable, Receiver);
            }
        }

        public override void OnGraphStop(Playable playable)
        {                                   
            RemoveNotification(playable);
            
            _IsFirstFrameHappened = false;
            Finished();
        }
        
        public void Finished()
        {                        
            for (int i = 0; i < _VFXList.Count; ++i)
            {
                var vfx = _VFXList[i];
                if(vfx == null)
                    continue;
                
                vfx.gameObject.SetActive(false);
                
                vfx.parent = _DefaultParent;
                vfx.localPosition = Vector3.zero;
                vfx.localRotation = Quaternion.identity;              
                vfx.localScale = Vector3.one;           
            }
            _Key = "";
            
            _DefaultParent = null;
            
            _VFXList.Clear();
        }
    }
}