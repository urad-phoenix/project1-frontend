namespace Phoenix.Playables
{
    using UnityEngine;
    using UnityEngine.Playables;
    using UnityEngine.Timeline;
    using Attribute;
    
    [Binding(typeof(IMaterialBinding), BindingCategory.Material)]
    [TrackColor(0.855f, 0.8623f, 0.87f)]
    [TrackClipType(typeof(StateColorClip))]
    [TrackBindingType(typeof(GameObject))]
    public class StateColorTrack : TrackAsset
    {
        public BindingTrackType BindingType;
        public string Key;
        public bool IsFinishReset = true;
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<StateColorBehaviour>.Create(graph, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
#if UNITY_EDITOR
            GameObject trackBinding = director.GetGenericBinding(this) as GameObject;
            if (trackBinding == null)
                return;
            //driver.AddFromName<Renderer>(trackBinding.gameObject, "transform");


#endif
            base.GatherProperties(director, driver);
        }
    }
}
