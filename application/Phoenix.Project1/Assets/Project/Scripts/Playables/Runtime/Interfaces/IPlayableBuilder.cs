namespace Phoenix.Playables
{
    using System;
    
    public interface IPlayableBuilder : IDisposable
    {
        void Build(params object[] data);
    }    
}