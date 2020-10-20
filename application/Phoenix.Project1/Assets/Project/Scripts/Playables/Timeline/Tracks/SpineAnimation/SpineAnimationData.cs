using UnityEngine.Playables;

namespace Phoenix.Playables
{
    public class SpineAnimationData : PlayableBehaviour
    {
        public string Name;
        public int Track;
        public float StartBlendingTime;
        public float EndBlendingTime;
        public float Duration;
        public bool IsLoop;
        public bool IsFirstFrameHappened;
        public string ReturnName;
        public bool IsReturnToSpecifyState;
    }    
}
