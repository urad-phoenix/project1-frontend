namespace Phoenix.Playables
{
    using Attribute;
    using UnityEngine;
    using UnityEngine.Playables;
    using System;
    using Cinemachine;
    using UnityEngine.Timeline;
    
    [Binding(typeof(ICameraBinding), BindingCategory.Camera)]
    [Serializable]
    [TrackClipType(typeof(CameraShotClip))]
    [TrackBindingType(typeof(CinemachineBrain))]
    [TrackColor(0.53f, 0.0f, 0.08f)]
    public class CameraShotTrack : TrackAsset
    {        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {            
            foreach (var c in GetClips())
            {
                CameraShotClip shot = (CameraShotClip) c.asset;
                CinemachineVirtualCameraBase vcam = shot.VirtualCamera.Resolve(graph.GetResolver());
                if (vcam != null)
                    c.displayName = vcam.Name;
            }

            var mixer = ScriptPlayable<CameraShotBehaviour>.Create(graph);
            mixer.SetInputCount(inputCount);
            return mixer;
        }	
    }
}