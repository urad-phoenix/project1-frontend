using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables
{
    public class EventBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int count = playable.GetInputCount();

            for (int i = 0; i < count; ++i)
            {
                var data = ((ScriptPlayable<EventBehaviourData>) playable.GetInput(i)).GetBehaviour();

                float weight = playable.GetInputWeight(i);

                if (data == null)
                    continue;

                if (/*playable.GetPlayState() == PlayState.Playing && */weight > 0.0f)
                {
                    if (!data.IsTrigger)
                    {
                        data.IsTrigger = true;

                        if (data.EventStrategy != null)
                        {
                            data.EventStrategy.Execute();
                            data.EventStrategy = null;
                        }
#if UNITY_EDITOR                        
                        if(!Application.isPlaying)
                            Debug.Log("Skill Event Trigger: " + i);
#endif
                    }
                }
                else
                {
                    data.IsTrigger = false;                
                }
            }
        }
    }
}