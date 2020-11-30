namespace Phoenix.Playables
{
    using UnityEngine.Playables;

    public class AnimatorBehaviourData : PlayableBehaviour
    {
        public string StateKey;
        public int Layer;
        public float StartBlendingTime;
        public float EndBlendingTime;
        public float AnimationLength;
        public bool IsLoop;
        public bool IsFirstFrameHappened;
        public string ReturnKey;
        public bool IsReturnToSpecifyState;
    }
}
