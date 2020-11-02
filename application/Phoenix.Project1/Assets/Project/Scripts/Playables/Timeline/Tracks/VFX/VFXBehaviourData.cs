namespace Phoenix.Playables
{
    using Phoenix.Playables.Utilities;
    using UnityEngine;
    using UnityEngine.Playables;
    
    public class VFXBehaviourData : PlayableBehaviour
    {
        public Transform LaunchPoint;
        public Transform TargetPoint;
        public bool IsFirstFrameHappened;        
        public float Duration;
        public Projectile Projectile;
        public string Key;
        public bool IsProjectile;        
        public AnimationCurve Curve;
        public float HeightScale = 1.84f;    
        public float AxisOffset = 0;
        public bool IsAnchor;
        public Transform VFX;
        
        public Transform DefaultParent;
        public Vector3 DistanceToTarget;
        public Vector3 DefaultPosition;           
        public Quaternion DefaultRotation;

        public VFXBehaviour.ParticleData ParticleData;

        public bool IsAnchorToEndPoint;
        
        public bool IsSetEndToVfxHitDummy;
        
        public string RuntimeKey;
        
        private void SetParticleSystem()
        {
            if (!Application.isPlaying)
            {
                ParticleData = new VFXBehaviour.ParticleData();
                ParticleData.ParticleSystem = VFX.GetComponent<ParticleSystem>();
                ParticleData.Initialize();
            }
        }

        public void Init()
        {          
            if(VFX == null)
                return;                                    
            
            VFX.gameObject.SetActive(true);   
            
            SetParticleSystem();
            
            DefaultParent = VFX.parent;           
            
            if (IsAnchor)
            {
                VFX.parent =LaunchPoint;
            }
            else
            {
                VFX.parent = null;
            }         
            
            VFX.localScale = Vector3.one;                       

            Projectile = VFX.GetComponent<Projectile>();
            if (Projectile != null)
            {
                if (LaunchPoint != null)
                {
                    VFX.position = LaunchPoint.position;
                    VFX.rotation = LaunchPoint.rotation;
                }

                Projectile.start = LaunchPoint;
               Projectile.target = TargetPoint;
               Projectile.duration = Duration;
               Projectile.isTimeline = true;
               Projectile.bullet = VFX.gameObject;
                
                if (IsProjectile)
                {
                   Projectile.Initializ();
                }
            }
            else
            {              
                if (LaunchPoint != null)
                {
                    VFX.rotation = LaunchPoint.rotation;
                    VFX.position = LaunchPoint.position;
                }
                
                if (IsAnchorToEndPoint)
                {
                    if (TargetPoint != null)
                    {
                        VFX.position = TargetPoint.position;
                        VFX.localRotation = TargetPoint.rotation;
                    }                
                }
                else if(IsAnchor)
                {
                    VFX.localPosition = Vector3.zero;
                    VFX.localRotation = Quaternion.identity;
                }

                if(LaunchPoint != null)                    
                    VFX.LookAt(LaunchPoint.position);
                
                VFX.localEulerAngles += new Vector3(0, 0, AxisOffset);               
                DefaultPosition = VFX.position;
                DefaultRotation = VFX.rotation;       
                
                DistanceToTarget = TargetPoint == null
                    ? Vector3.zero
                    : TargetPoint.position - VFX.position;                               
            }
        }   
        
        public void Finished()
        {
            if(VFX == null)
                return;
            
            VFX.gameObject.SetActive(false); 
            VFX.parent = DefaultParent;
            VFX.localPosition = Vector3.zero;
            VFX.localRotation = Quaternion.identity;
            VFX.localScale = Vector3.one;            
            //VFX.gameObject.SetActive(false);
            //if (Application.isPlaying && Entry.IsInitialized && !string.IsNullOrEmpty(RuntimeKey))
            //{
            //    var poolManager = Entry.GetModule<PoolManager>();
            //    if (poolManager.IsContainsPool(RuntimeKey))
            //    {
            //        poolManager.Recycle(RuntimeKey, VFX.gameObject);                   
            //    }
            //    else
            //    {
            //        Debug.LogError("VFX key no found " + RuntimeKey);
            //    }                
            //}
        }

        /* private float m_LastTime = -1f;
         private uint m_RandomSeed = 1;
         private const float kUnsetTime = -1f;
         private float m_SystemTime;
         
         public ParticleSystem particleSystem { get; private set; }
         
         public static ScriptPlayable<VFXBehaviourData> Create(PlayableGraph graph, ParticleSystem component, uint randomSeed)
         {
             if ((UnityEngine.Object) component == (UnityEngine.Object) null)
                 return ScriptPlayable<VFXBehaviourData>.Null;
             ScriptPlayable<VFXBehaviourData> scriptPlayable = ScriptPlayable<VFXBehaviourData>.Create(graph, 0);
             scriptPlayable.GetBehaviour().Initialize(component, randomSeed);
             return scriptPlayable;
         }
         
         public void Initialize(ParticleSystem ps, uint randomSeed)
         {
             this.m_RandomSeed = Math.Max(1U, randomSeed);
             this.particleSystem = ps;
             this.m_SystemTime = 0.0f;
             this.SetRandomSeed();
         }
 
         private void SetRandomSeed()
         {
             this.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
             ParticleSystem[] componentsInChildren =
                 this.particleSystem.gameObject.GetComponentsInChildren<ParticleSystem>();
             uint randomSeed = this.m_RandomSeed;
             foreach (ParticleSystem particleSystem in componentsInChildren)
             {
                 if (particleSystem.useAutoRandomSeed)
                 {
                     particleSystem.useAutoRandomSeed = false;
                     particleSystem.randomSeed = randomSeed;
                     ++randomSeed;
                 }
             }
         }*/
    }
}