namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;

    public class TimeDilationBehaviour : PlayableBehaviour
    {
        readonly float defaultTimeScale = 1f;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            int inputCount = playable.GetInputCount();

            float mixedTimeScale = 0f;
            float totalWeight = 0f;
            int currentInputCount = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0f)
                    currentInputCount++;

                totalWeight += inputWeight;

                ScriptPlayable<TimeDilationBehaviourData> playableInput = (ScriptPlayable<TimeDilationBehaviourData>)playable.GetInput(i);
                TimeDilationBehaviourData input = playableInput.GetBehaviour();

                mixedTimeScale += inputWeight * input.timeScale;
            }

            Time.timeScale = mixedTimeScale + defaultTimeScale * (1f - totalWeight);

            if (currentInputCount == 0)
                Time.timeScale = defaultTimeScale;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Time.timeScale = defaultTimeScale;
        }

        public override void OnGraphStop(Playable playable)
        {
            Time.timeScale = defaultTimeScale;
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Time.timeScale = defaultTimeScale;
        }
    }
}
