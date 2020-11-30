using System.Collections.Generic;

namespace Phoenix.Project1.Game
{
    public interface IConfigurationDatabase
    {
        IEnumerable<T> Query<T>();
    }
}
