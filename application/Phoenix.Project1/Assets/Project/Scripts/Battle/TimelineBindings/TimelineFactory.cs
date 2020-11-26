using System;
using Phoenix.Playables;
using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public static partial class TimelineBinding
    {
        public static IObservable<PlayableDirector> PlayableAsObservable(ActorFrameMotion actData, BattleController controller)
        {
            try
            {
                var motion = actData.MotionId;

                var director = controller.GetPlayableDirector(motion, actData.ActorId);

                //Debug.Log($"act key {actData.MotionId}");

                if (director == null || director.playableAsset == null)
                {
                    //Debug.LogError($"act key {actData.MotionId} timeline is null"); 
                    return new Subject<PlayableDirector>();
                }

                var tracks = ((TimelineAsset) director.playableAsset).GetOutputTracks();

                foreach (var track in tracks)
                {
                    if (track is SpineAnimationTrack)
                    {
                        BindSpineTrack(director, actData, track, controller);
                    }
                    else if (track is VFXTrack)
                    {
                        BindVFXTrack(director, actData, track, controller);
                    }
                    else if (track is CameraShotTrack)
                    {
                        BindCameraTrack(director, actData, track, controller);
                    }
                    else if (track is AudioTrack)
                    {
                        BindAudioTrack(director, track, controller);
                    }
                    else if (track is TransformTweenTrack)
                    {
                        if(actData.TargetLocation == 0)
                            continue;
                        BindTransformTrack(director, actData, track, controller);
                    }
                }

                return director.PlayAsObservable(new CompositeDisposable());
            }
            catch (Exception e)
            {
                Debug.LogError($"action {actData.MotionId} {actData.ActorId} {actData.TargetLocation} \n {e}");              
                return new Subject<PlayableDirector>();
            }
        }
    }
}