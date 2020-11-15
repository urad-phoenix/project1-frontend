using System.Collections.Generic;
using System.Linq;
using Phoenix.Playables;
using Spine.Unity;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public class SpineAnimationBinding : ISpineAnimationBinding
    {
        public void Bind(PlayableDirector playableDirector, params object[] data)
        {   
            var actorData = data[0] as ActData;
            
            if(actorData == null)
                return;

            var tracks = data[1] as IEnumerable<SpineAnimationTrack>;                        
            
            if(tracks == null || !tracks.Any())
                return;

            var controller = data[2] as BattleController;

            foreach (var track in tracks)
            {
                var avatar = controller?.GetAvatar(actorData.ActorId);                                   

                var anim = avatar?.GetComponent<SkeletonAnimation>();
            
                playableDirector.SetGenericBinding(track, anim);    
            }                                                
        }        
    }
}