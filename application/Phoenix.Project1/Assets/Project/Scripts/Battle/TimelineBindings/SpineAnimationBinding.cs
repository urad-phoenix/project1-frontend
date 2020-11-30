using Phoenix.Playables;
using Phoenix.Project1.Common.Battles;
using Spine.Unity;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public static partial class TimelineBinding
    {
        public static void BindSpineTrack(PlayableDirector playableDirector, params object[] data)
        {   
            var actorData = data[0] as ActorFrameMotion;
            
            if(actorData == null)
                return;

//            var tracks = data[1] as IEnumerable<SpineAnimationTrack>;                        
//            
//            if(tracks == null || !tracks.Any())
//                return;

            var track = data[1] as SpineAnimationTrack;

            if(track == null)
                return;
            
            var controller = data[2] as BattleController;
                       
            var avatar = controller?.GetAvatarByID(actorData.ActorId);                                   

            var anim = avatar?.GetComponent<SkeletonAnimation>();
        
            playableDirector.SetGenericBinding(track, anim);                                                            
        }        
    }
}