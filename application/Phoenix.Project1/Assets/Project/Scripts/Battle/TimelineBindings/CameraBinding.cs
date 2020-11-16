using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Phoenix.Playables;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public class CameraBinding : ICameraBinding
    {
        public void Bind(PlayableDirector playableDirector, params object[] data)
        {
            var actorData = data[0] as ActData;
            
            if(actorData == null)
                return;

//            var tracks = data[1] as IEnumerable<CameraShotTrack>;                        
            
//            if(tracks == null || !tracks.Any())
//                return;

            var track = data[1] as CameraShotTrack;
            
            if(track == null)
                return;
            
            var controller = data[2] as BattleController;

            var camera = controller?.GetMainCamera();

            var brain = camera.GetComponent<CinemachineBrain>();
            
            playableDirector.SetGenericBinding(track, brain);

            foreach (var clip in track.GetClips())
            {
                var c = clip.asset as CameraShotClip;

                var virtualCamera = controller.GetVirtualCamera(c.BindingType == BindingTrackType.Actor || c.BindingType == BindingTrackType.Target, c.Key);

                c.VirtualCamera = new ExposedReference<CinemachineVirtualCameraBase>()
                {
                    defaultValue = virtualCamera 
                };
            }                        
        }
    }
}