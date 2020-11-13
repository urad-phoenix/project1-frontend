using Phoenix.Playables;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public class TransformBinding : ITransformBinding
    {
        public void Bind(PlayableDirector playableDirector, params object[] data)
        {
            var track = data[0] as TransformTweenTrack;
        }
    }
}