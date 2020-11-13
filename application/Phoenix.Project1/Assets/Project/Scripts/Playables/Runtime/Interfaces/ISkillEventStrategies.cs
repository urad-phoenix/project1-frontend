namespace Phoenix.Playables
{
    public interface ISkillEventStrategies
    {
        void Execute();
        int GetSeqence();
        int GetOrder();
    }
}