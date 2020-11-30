using System.Collections.Generic;
using System.Linq;
using Phoenix.Playables;
using Phoenix.Project1.Common.Battles;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public static partial class TimelineBinding
    {
        public static void BindTransformTrack(PlayableDirector playableDirector, params object[] data)
        {
            var actData = data[0] as ActorFrameMotion;
             
            if(actData == null)
                return;

            var track = data[1] as TransformTweenTrack;           
            
            if(track == null)
                return;     

            var controller = data[2] as BattleController;

            var role = controller.GetRole(actData.ActorId);

            //var location =             

            var target = controller.GetLocator(actData.TargetLocation);
            
            var startBindingObject = track.StartBindingType == BindingTrackType.Actor ? role.GetAvatar()?.transform : target.transform;
        
            var targetBindingObject = track.EndBindingType == BindingTrackType.Actor ? role.GetAvatar()?.transform : target.transform;

            var clips = track.GetClips();

            foreach (var clip in clips)
            {
                var c = clip.asset as TransformTweenClip;

                c.startLocation = new ExposedReference<Transform>()
                {
                    defaultValue = startBindingObject,
                };

                c.endLocation = new ExposedReference<Transform>()
                {
                    defaultValue = targetBindingObject
                };
            }
            playableDirector.SetGenericBinding(track, startBindingObject);                            
        }               
    }
}