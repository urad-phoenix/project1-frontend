namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using Cinemachine;
    using UnityEngine.Timeline;
    using System;
    
    [Serializable]
    public class CameraShotBehaviourData : PlayableBehaviour
    {
        public CinemachineVirtualCameraBase VirtualCamera;
        public bool IsFirstFrameHappened;
        public bool IsValid { get { return VirtualCamera != null; } }
        //public CameraShotClip.NoiseSetting CameraNoiseSetting;
        public NoiseSettings NoiseSettings;
        public float AmplitudeGain;
        public float FrequencyGain;
        public bool IsOpenNoise;
        public CinemachineBasicMultiChannelPerlin ChannelPerlin;
        
        public void Init()
        {
            if(VirtualCamera == null)
                return;
            
            var camera = VirtualCamera as CinemachineVirtualCamera;

            ChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            
            if (IsOpenNoise)
            {
                if (ChannelPerlin == null)
                {
                    ChannelPerlin = camera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();                   
                }                         
            }            
        }

        public void Start()
        {
            if (ChannelPerlin != null)
            {
                ChannelPerlin.m_NoiseProfile = NoiseSettings;
                ChannelPerlin.m_AmplitudeGain = AmplitudeGain;
                ChannelPerlin.m_FrequencyGain = FrequencyGain;
            }
        }

        public void Reset()
        {          
            if (ChannelPerlin != null)
            {
                ChannelPerlin.m_NoiseProfile = null;
                ChannelPerlin.m_AmplitudeGain = 0;
                ChannelPerlin.m_FrequencyGain = 0;
            }
        }
    }
    
    public class CameraShotClip : PlayableAsset, IPropertyPreview
    {
        //private CameraShotBehaviourData m_ShotBehaviourData = new CameraShotBehaviourData();
        public ExposedReference<CinemachineVirtualCameraBase> VirtualCamera;

        public BindingTrackType BindingType;

        public int Key;

        [HideInInspector]
        public bool IsOpenNoise;
        
        [Serializable]
        public class NoiseSetting
        {
            public NoiseSettings NoiseSettings;
            public float AmplitudeGain;
            public float FrequencyGain;
        }
        
        [HideInInspector]
        public NoiseSetting CameraNoiseSetting;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<CameraShotBehaviourData>.Create(graph);
            
            var clone = playable.GetBehaviour();
            clone.VirtualCamera = VirtualCamera.Resolve(graph.GetResolver());
            clone.NoiseSettings = CameraNoiseSetting.NoiseSettings;
            clone.AmplitudeGain = CameraNoiseSetting.AmplitudeGain;
            clone.FrequencyGain = CameraNoiseSetting.FrequencyGain;
            clone.IsOpenNoise = IsOpenNoise;
            clone.IsFirstFrameHappened = false;
            clone.Init();
            //var camera = clone.VirtualCamera as CinemachineVirtualCamera;

            /*var channel = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (channel == null)
            {
                channel = camera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }

            channel.m_NoiseProfile = CameraNoiseSetting.NoiseSettings;
            channel.m_AmplitudeGain = CameraNoiseSetting.AmplitudeGain;
            channel.m_FrequencyGain = CameraNoiseSetting.FrequencyGain;*/
            //channel.m_NoiseProfile = new NoiseSettings();
            //camera.
            return playable;
        }
        

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            driver.AddFromName<Transform>("m_LocalPosition.x");
            driver.AddFromName<Transform>("m_LocalPosition.y");
            driver.AddFromName<Transform>("m_LocalPosition.z");
            driver.AddFromName<Transform>("m_LocalRotation.x");
            driver.AddFromName<Transform>("m_LocalRotation.y");
            driver.AddFromName<Transform>("m_LocalRotation.z");

            driver.AddFromName<Camera>("field of view");
            driver.AddFromName<Camera>("near clip plane");
            driver.AddFromName<Camera>("far clip plane");
        }      
    }
}