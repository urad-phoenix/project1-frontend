namespace Phoenix.Playables
{
    using System;
    using UnityEngine;
    using UnityEngine.Playables;

    [Serializable]
    public class StateColorBehaviourData : PlayableBehaviour
    {
        public bool UseTintColor = false;
        public Color Color = Color.white;
        public bool UseEffectGain = true;
        public float EffectGain = 0;
        public bool UseDissolveControl = true;
        public float DissolveControl = 0;
        public bool UseShadowColor = false;
        public Color ShadowColor = Color.gray;
        public bool UseMapColor;
        public Color MapColor = Color.white;
        public Shader SpShader;
        public Color FresnelColor = new Vector4(1, 0.47f, 0.0f, 1);
        //new Vector4(1, 0.7f, 0.0f, 1)
        [HideInInspector]
        public bool IsFinishReset;


        public override void OnPlayableCreate(Playable playable)
        {

        }
    }
}
