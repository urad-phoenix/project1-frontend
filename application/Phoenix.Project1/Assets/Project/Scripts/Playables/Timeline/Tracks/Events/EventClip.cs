using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables
{
    [Serializable]
    public class EventClip : PlayableAsset
    {         
        [HideInInspector]
        public EventBehaviourData m_SkillData = new EventBehaviourData();

        [HideInInspector]
        public ISkillEventStrategies EventStrategy;

        [HideInInspector]
        public int Seqence;

        [HideInInspector]
        public int Order;                
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<EventBehaviourData>.Create(graph);

            var data = playable.GetBehaviour();

            data.EventStrategy = m_SkillData.EventStrategy;
            data.IsTrigger = false;

           // data.EventStrategy = EventStrategy;
            return playable;
        }
    }
}