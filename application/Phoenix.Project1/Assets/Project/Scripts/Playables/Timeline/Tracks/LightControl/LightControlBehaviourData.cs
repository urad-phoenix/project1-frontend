namespace Phoenix.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Playables;

    [Serializable]
    public class LightControlBehaviourData : PlayableBehaviour
    {
        public Color color = Color.white;
        public float intensity = 1f;
        public float bounceIntensity = 1f;
        public float range = 10f;
    }
}
