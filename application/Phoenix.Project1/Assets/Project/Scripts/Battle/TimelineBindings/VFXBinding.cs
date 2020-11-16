using System.Collections.Generic;
using System.Linq;
using Phoenix.Playables;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public class VFXBinding : IVFXBinding
    {
        public void Bind(PlayableDirector playableDirector, params object[] data)
        {
            var actorData = data[0] as ActData;
            
            if(actorData == null)
                return;

//            var tracks = data[1] as IEnumerable<VFXTrack>;                        
//            
//            if(tracks == null || !tracks.Any())
//                return;

            var track = data[1] as VFXTrack;

            if(track == null)
                return;
            
            var controller = data[2] as BattleController;
                        
       
            var avatar = controller?.GetAvatarByID(actorData.ActorId);

            var startLoaction = track.StartBindingType == BindingTrackType.Actor
                ? controller.GetAvatarByID(actorData.ActorId)?.GetDummy(track.AnchorDummyKey)
                : controller.GetLocator(actorData.Location)?.transform;

            var endLoaction = track.StartBindingType == BindingTrackType.Actor
                ? controller.GetAvatarByID(actorData.ActorId)?.GetDummy(track.AnchorDummyKey)
                : controller.GetLocator(actorData.Location)?.transform;
                
            foreach (var clip in track.GetClips())
            {
                var vfxClip = clip.asset as VFXClip;

                vfxClip.RuntimeKey = track.VFXKey;
                
                var vfx = avatar?.GetVFX(track.VFXKey);

                if (vfx)
                {
                    vfxClip.VFX = new ExposedReference<Transform>()
                    {
                        defaultValue = vfx.transform
                    };    
                }                    
                
                vfxClip.LaunchPoint = new ExposedReference<Transform>()
                {
                    defaultValue = startLoaction,
                };

                vfxClip.TargetPoint = new ExposedReference<Transform>()
                {
                    defaultValue = endLoaction
                };                  
            }     
        }
    }
}