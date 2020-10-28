namespace Phoenix.Project1.Client.Battles
{
    public interface IStateBehaviour
    {
        void Star(StateBinding binding);

        void Stop(StateBinding binding);

        void Update(StateBinding binding);
    }
}