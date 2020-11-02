namespace Phoenix.Project1.Client.Battles
{
    public interface IStateBehaviour
    {
        void Start(StateBinding binding);

        void Stop(StateBinding binding);

        void Update(StateBinding binding);
    }
}