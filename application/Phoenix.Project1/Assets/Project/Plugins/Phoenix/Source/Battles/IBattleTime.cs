namespace Phoenix.Project1.Battles
{
    public interface IBattleTime
    {
        int Frame { get; }
        
        int Advance();
    }
}
