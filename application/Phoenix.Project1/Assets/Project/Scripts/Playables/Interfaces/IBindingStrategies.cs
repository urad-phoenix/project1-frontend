namespace Phoenix.Playables
{
    using UnityEngine.Playables;
    
    public interface IBindingStrategies
    {
        void Bind(PlayableDirector playableDirector, params object[] data);
    }
}