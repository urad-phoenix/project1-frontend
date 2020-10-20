namespace Phoenix.Playables
{
    public interface ITrackRuntimeBinding
    {
        UnityEngine.Object GetBindingKey();
        BindingCategory GetBindingType();
        BindingTrackType GetTrackType();
    }
}