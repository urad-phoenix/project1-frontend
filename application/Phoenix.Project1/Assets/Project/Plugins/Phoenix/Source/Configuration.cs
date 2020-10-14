using Phoenix.Project1.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Project1.Game
{
    public interface IConfigurationDatabase
    {
        IEnumerable<T> Query<T>();
    }

    public class Configuration : IConfigurationDatabase
    {
        public readonly Regulus.RelationalTables.Database Database;
        public readonly BuffInfo[] BuffInfos;
        public readonly Localization[] Localizations;

        public Configuration(Regulus.RelationalTables.Database database)
        {
            Database = database;
            //BuffInfos = Database.Query<BuffInfo>().ToArray();
            Localizations = Database.Query<Localization>().ToArray();
        }

        public IEnumerable<T> Query<T>()
        {
            return Database.Query<T>();
        }
    }
}
