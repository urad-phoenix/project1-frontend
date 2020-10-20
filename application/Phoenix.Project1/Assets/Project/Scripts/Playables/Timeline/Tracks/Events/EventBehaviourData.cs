using System;
using UnityEngine.Playables;

namespace Phoenix.Playables
{
    [Serializable]
    public class EventBehaviourData : PlayableBehaviour
    {
        public bool IsTrigger;

        public ISkillEventStrategies EventStrategy;
    }
}