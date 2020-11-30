using Phoenix.Project1.Client.Audio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public static partial class TimelineBinding
    {
        public static void BindAudioTrack(PlayableDirector playableDirector, params object[] data)
        {           
            var track = data[0] as AudioTrack;
            
            if(track == null)
                return;
            
            var volume = AudioManager.Instance.GetSoundVolume();
            
            track.CreateCurves("nameOfAnimationClip");
            
            track.curves.SetCurve(string.Empty, typeof(AudioTrack), "volume", AnimationCurve.Linear(0, volume, 0, volume));                                          
        }
    }
}