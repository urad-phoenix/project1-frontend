using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IConfigurationDatabase
{
    IEnumerable<T> Query<T>();
}

public class Configuration : IConfigurationDatabase
{
    public readonly Regulus.RelationalTables.Database Database;
    public readonly Phoenix.Project1.Configs.Localization[] Localizations;

    public Configuration(Regulus.RelationalTables.Database database)
    {
        Database = database;
        //BuffInfos = Database.Query<BuffInfo>().ToArray();
        Localizations = Database.Query<Phoenix.Project1.Configs.Localization>().ToArray();
    }

    public IEnumerable<T> Query<T>()
    {
        return Database.Query<T>();
    }
}
