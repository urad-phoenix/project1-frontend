using Phoenix.Playables.Attribute;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables
{
    [TrackBindingType(typeof(GameObject))]
    [Binding(typeof(IVFXBuffBinding), BindingCategory.BuffVFX)]
    [TrackClipType(typeof (VFXBuffClip))]
    [TrackColor(0.2f, 0.4866645f, 0.2f)]
    public class VFXBuffTrack :  TrackAsset, ITrackRuntimeBinding
    {
        public string BuffVFXKey;

        public int DummyType;

        public bool IsRestart;
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var c = clip.asset as VFXBuffClip;
                c.Key = BuffVFXKey;
                c.IsRestart = IsRestart;
            }
            return ScriptPlayable<VFXBuffBehaviour>.Create(graph, inputCount);
        }

        public Object GetBindingKey()
        {
            return null;
        }

        public BindingCategory GetBindingType()
        {
            return BindingCategory.BuffVFX;
        }

        public BindingTrackType GetTrackType()
        {
            return BindingTrackType.Actor;
        }
    }
}