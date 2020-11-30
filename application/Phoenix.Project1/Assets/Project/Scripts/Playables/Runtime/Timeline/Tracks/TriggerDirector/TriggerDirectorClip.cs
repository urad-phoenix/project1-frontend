namespace Phoenix.Playables
{
    using UnityEngine.Timeline;
    using UnityEngine;
    using UnityEngine.Playables;

    [System.Serializable]
    public class TriggerDirectorClip : PlayableAsset
    {        
        private readonly TriggerDirectorData template = new TriggerDirectorData();             
        
        public TimelineAsset TimelineAsset;

        public ExposedReference<PlayableDirector> Director;
        
        public string AssetKey;
        
        public override double duration
        {
            get
            {
                if (TimelineAsset == null)
                    return base.duration;

                double length = TimelineAsset.duration;
                if (length < float.Epsilon)
                    return base.duration;
                
                return length;
            }           
        }
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var data = ScriptPlayable<TriggerDirectorData>.Create(graph, template);
            
            var clone = data.GetBehaviour();
            
            clone.Director = Director.Resolve(graph.GetResolver());
            
            if (clone.Director != null && clone.Director.playableAsset != null)
            {
                if (TimelineAsset != null)
                {
                    clone.Director.playableAsset = TimelineAsset;
                }               
            }
            
            return data;
        }
    }
}