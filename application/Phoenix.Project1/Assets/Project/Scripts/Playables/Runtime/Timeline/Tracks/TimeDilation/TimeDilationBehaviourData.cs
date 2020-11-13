namespace Phoenix.Playables
{
    using System;
    using UnityEngine.Playables;

    [Serializable]
    public class TimeDilationBehaviourData : PlayableBehaviour
    {
        public float timeScale = 1f;
    }
}