namespace Phoenix.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;

    [Serializable]
    public class StateColorClip : PlayableAsset, ITimelineClipAsset
    {
        public StateColorBehaviourData template = new StateColorBehaviourData();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<StateColorBehaviourData>.Create(graph, template);
            //StateColorBehaviour clone = playable.GetBehaviour ();
            return playable;
        }
    }
}
