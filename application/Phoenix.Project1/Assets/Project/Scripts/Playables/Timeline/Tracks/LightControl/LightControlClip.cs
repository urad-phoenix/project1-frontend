namespace Phoenix.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class LightControlClip : PlayableAsset, ITimelineClipAsset
    {
        public LightControlBehaviourData template = new LightControlBehaviourData();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LightControlBehaviourData>.Create(graph, template);
            return playable;
        }
    }
}
