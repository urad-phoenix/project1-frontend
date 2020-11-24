using System.Security.Policy;
using Phoenix.Playables;
using Phoenix.Project1.Client.Audio;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Client.Battles
{
    public class AudioBinding
    {
        public void Bind(PlayableDirector playableDirector, params object[] data)
        {
            var actorData = data[0] as ActData;
            
            if(actorData == null)
                return;

            var track = data[1] as AudioTrack;
            
            if(track == null)
                return;
            
            var volume = AudioManager.Instance.GetSoundVolume();
            
            track.CreateCurves("nameOfAnimationClip");
            
            track.curves.SetCurve(string.Empty, typeof(AudioTrack), "volume", AnimationCurve.Linear(0, volume, 0, volume));                                          
        }
    }
}