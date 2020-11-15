using System.Collections.Generic;
using System.Linq;
using Phoenix.Playables;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public class TransformBinding : ITransformBinding
    {
        public void Bind(PlayableDirector playableDirector, params object[] data)
        {
            var actData = data[0] as MoveData;
             
            if(actData == null)
                return;

            var tracks = data[1] as IEnumerable<TransformTweenTrack>;           
            
            if(tracks == null || !tracks.Any())
                return;     

            var controller = data[2] as BattleController;

            var role = controller.GetRole(actData.MoveActorId);

            var target = controller.GetLocator(actData.Location);

            foreach (var track in tracks)
            {
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
}