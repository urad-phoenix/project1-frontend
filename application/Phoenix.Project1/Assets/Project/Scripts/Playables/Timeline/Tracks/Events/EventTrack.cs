using Phoenix.Playables.Attribute;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables
{
    [TrackClipType(typeof(EventClip))]
    [Binding(typeof(IEventBinding), BindingCategory.Event)]
    [TrackColor(0.0f, 0.0f, 1f)]
    public class EventTrack : TrackAsset, ITrackRuntimeBinding
    {
        public SkillEventType EventType;
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {           
            return ScriptPlayable<EventBehaviour>.Create(graph, inputCount);
        }

        public Object GetBindingKey()
        {
            return this;
        }

        public BindingCategory GetBindingType()
        {
            return BindingCategory.Event;
        }

        public BindingTrackType GetTrackType()
        {
            return BindingTrackType.None;
        }
    }
}