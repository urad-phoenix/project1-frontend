using System;
using System.Collections.Generic;

namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    
    public class VFXBehaviour : PlayableBehaviour
    {
        private bool m_FirstFrame;

        //private Transform m_Binding;

        private bool m_IsFirstFrameHappened;

        //private Quaternion m_Rotation;

        //private Transform m_Parent;

        //private Vector3 m_Position;
        
        private float m_LastTime = -1f;
        private uint m_RandomSeed = 1;
        private const float kUnsetTime = -1f;
        private float m_SystemTime;
        private string m_Key;
        private List<Transform> m_VFXList = new List<Transform>();
        private Transform m_DefaultParent;
        
        public class ParticleData
        {
            public ParticleSystem ParticleSystem;
            public float RandomSeed;
            private float m_LastTime = -1f;
            private uint m_RandomSeed = 1;
            private const float kUnsetTime = -1f;
            private float m_SystemTime;
            
            public void Simulate(float time)
            {
                if ((UnityEngine.Object) ParticleSystem != (UnityEngine.Object) null)
                {
                    // float time = (float) playable.GetTime<Playable>();
                    if (!Mathf.Approximately(m_LastTime, -1f) && Mathf.Approximately(m_LastTime, time))
                        return;
                    float num1 = Time.fixedDeltaTime * 0.5f;
                    float t1 = time;
                    float num2 = t1 - this.m_LastTime;
                    float num3 =ParticleSystem.main.startDelay.Evaluate((float) ParticleSystem.randomSeed);
                    float num4 = ParticleSystem.main.duration + num3;
                    float num5 = (double) t1 <= (double) num4 ? this.m_SystemTime - num3 : this.m_SystemTime;
                    if ((double) t1 < (double) this.m_LastTime || (double) t1 < (double) num1 ||
                        Mathf.Approximately(this.m_LastTime, -1f) ||
                        (double) num2 > (double) ParticleSystem.main.duration ||
                        (double) Mathf.Abs(num5 - ParticleSystem.time) >= (double) Time.maximumParticleDeltaTime)
                    {
                        ParticleSystem.Simulate(0.0f, true, true);
                        ParticleSystem.Simulate(t1, true, false);
                        this.m_SystemTime = t1;
                    }
                    else
                    {
                        float num6 = (double) t1 <= (double) num4 ? num4 : ParticleSystem.main.duration;
                        float num7 = t1 % num6;
                        float t2 = num7 - this.m_SystemTime;
                        if ((double) t2 < -(double) num1)
                            t2 = num7 + num4 - this.m_SystemTime;
                        ParticleSystem.Simulate(t2, true, false);
                        this.m_SystemTime += t2;
                    }

                    this.m_LastTime = time;    
                } 
            }
            
            public void SetRandomSeed()
            {
                if (ParticleSystem == null)
                    return;
                ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                /*ParticleSystem[] componentsInChildren = m_ParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
                uint randomSeed = this.m_RandomSeed;
                foreach (ParticleSystem particleSystem in componentsInChildren)
                {
                    if (particleSystem.useAutoRandomSeed)
                    {
                        particleSystem.useAutoRandomSeed = false;
                        particleSystem.randomSeed = randomSeed;
                        ++randomSeed;
                    }
                }*/
            }
            
            public void Initialize()
            {
                m_SystemTime = 0.0f;
                SetRandomSeed();
            }
        }
        //public ParticleSystem m_ParticleSystem { get; private set; }
                
        public void Initialize(ParticleSystem ps, uint randomSeed)
        {
            this.m_RandomSeed = Math.Max(1U, randomSeed);
            //m_ParticleSystem = ps;
            this.m_SystemTime = 0.0f;
            this.SetRandomSeed(ps);
        }

        private void SetRandomSeed(ParticleSystem system)
        {
            if (system == null)
                return;
            system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            /*ParticleSystem[] componentsInChildren = m_ParticleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
            uint randomSeed = this.m_RandomSeed;
            foreach (ParticleSystem particleSystem in componentsInChildren)
            {
                if (particleSystem.useAutoRandomSeed)
                {
                    particleSystem.useAutoRandomSeed = false;
                    particleSystem.randomSeed = randomSeed;
                    ++randomSeed;
                }
            }*/
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {          
            var count =  playable.GetInputCount();

            if (!m_IsFirstFrameHappened)
            {
                m_IsFirstFrameHappened = true;
                m_Key = "";
            
                m_DefaultParent = null;
            
                m_VFXList.Clear();
            }

            for (int i = 0; i < count; ++i)
            {
                var weight = playable.GetInputWeight(i);

                var input = (ScriptPlayable<VFXBehaviourData>)playable.GetInput(i);

                var data = input.GetBehaviour();
                
                if (weight > 0)
                {
                    float time = (float) input.GetTime();
                                  
                    if(data.VFX == null)
                        continue;
                    
                    if (data.IsFirstFrameHappened == false)
                    {
                        if(string.IsNullOrEmpty(m_Key))
                            m_Key = data.RuntimeKey;
                        
                        if(m_DefaultParent == null)
                            m_DefaultParent = data.DefaultParent;
                                                
                        data.IsFirstFrameHappened = true;
                        data.Init();
                        m_VFXList.Add(data.VFX);
                    }

                    EditorMod(time, data);

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
                        m_VFXList.Remove(data.VFX);
                        data.Finished();
                    }
                }
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

        public override void OnGraphStop(Playable playable)
        {           
            m_IsFirstFrameHappened = false;
            Finished();
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            this.m_LastTime = -1f;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            this.m_LastTime = -1f;
        }
        
        public void Finished()
        {
            
            
            for (int i = 0; i < m_VFXList.Count; ++i)
            {
                var vfx = m_VFXList[i];
                if(vfx == null)
                    continue;
                
                vfx.gameObject.SetActive(false);
                
                vfx.parent = m_DefaultParent;
                vfx.localPosition = Vector3.zero;
                vfx.localRotation = Quaternion.identity;              
                vfx.localScale = Vector3.one;
               // vfx.gameObject.SetActive(false);
                
                //if (Application.isPlaying && Entry.IsInitialized)
                //{
                //    if (m_PoolManager == null)
                //    {
                //        m_PoolManager = Entry.GetModule<PoolManager>();
                //    }
                //    
                //    if (m_PoolManager.IsContainsPool(m_Key))
                //    {
                //        m_PoolManager.Recycle(m_Key, vfx.gameObject);                      
                //    }
                //    else
                //    {
                //        Debug.LogError("Finished VFX key not found " + m_Key);
                //    }    
                //}                
            }
            m_Key = "";
            
            m_DefaultParent = null;
            
            m_VFXList.Clear();
        }
    }
}