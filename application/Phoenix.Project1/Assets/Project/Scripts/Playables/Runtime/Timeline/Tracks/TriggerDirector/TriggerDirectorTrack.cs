namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Attribute;
    
    [Binding(typeof(ITriggerDirectorBinding), BindingCategory.Director)]
    [TrackColor(0.8f, 0.5f, 0.8f)]
    [TrackClipType(typeof(TriggerDirectorClip))]
    public class TriggerDirectorTrack : TrackAsset
    {
        public BindingTrackType BindingType;
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TriggerDirectorBehaviour>.Create(graph, inputCount);
        }
    }
}