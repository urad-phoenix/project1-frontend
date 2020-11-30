using System.Collections.Generic;

namespace Phoenix.Project1.Game
{
    public class FakeConfiguration : IConfigurationDatabase
    {
        // 這邊可以自行加入需要模擬的測試資料
        IEnumerable<T> IConfigurationDatabase.Query<T>() 
        {
            return new T[] { };
        }
    }
}
