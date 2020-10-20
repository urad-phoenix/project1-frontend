namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;

    public class TriggerDirectorBehaviour : PlayableBehaviour
    {
        private float m_NormalTime; 
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            int count = playable.GetInputCount();

            for (int i = 0; i < count; ++i)
            {
                var inputPlayable = (ScriptPlayable<TriggerDirectorData>) playable.GetInput(i);
                                
                var weight = playable.GetInputWeight(i);
                
                var behaviour = inputPlayable.GetBehaviour();
                
                if(behaviour.Director == null)
                    continue;
                
                var director = behaviour.Director; 
                
                if (director != null && weight > 0)
                {                   
                   
                    if (Application.isPlaying)
                    {
                        if (director.state != PlayState.Playing)
                            director.Play();
                    }
                    else
                    {
                        director.time = inputPlayable.GetTime();
                        director.Evaluate();    
                    }                   
                }
            }            
        }               

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
                return;
            
            int count = playable.GetInputCount();

            for (int i = 0; i < count; ++i)
            {
                var inputPlayable = (ScriptPlayable<TriggerDirectorData>) playable.GetInput(i);
                
                var behaviour = inputPlayable.GetBehaviour();
                
                if (behaviour.Director != null && behaviour.Director.state == PlayState.Playing)
                {
                    behaviour.Director.Pause();
                }                   
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
                return;
            int count = playable.GetInputCount();

            for (int i = 0; i < count; ++i)
            {
                var inputPlayable = (ScriptPlayable<TriggerDirectorData>) playable.GetInput(i);
                               
                var behaviour = inputPlayable.GetBehaviour();

                if (behaviour.Director != null && behaviour.Director.state == PlayState.Paused)
                {
                    behaviour.Director.Resume();
                }
            }
        }       
    }
}