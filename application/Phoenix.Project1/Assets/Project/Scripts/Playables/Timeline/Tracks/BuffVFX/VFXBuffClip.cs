using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables
{
    [System.Serializable]
    public class VFXBuffClip : PlayableAsset
    {
        private VFXBuffData template;
        
        [HideInInspector] public string Key;

        [HideInInspector] public bool IsRestart;
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<VFXBuffData>.Create(graph, template);

            var data = playable.GetBehaviour();

            data.IsFirstFrameHappened = false;
            data.Key = Key;
            data.IsRestart = IsRestart;
            return playable;
        }
    }
}