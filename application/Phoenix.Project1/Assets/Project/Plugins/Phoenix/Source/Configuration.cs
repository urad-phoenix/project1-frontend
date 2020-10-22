﻿using Phoenix.Project1.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phoenix.Project1.Game
{
    public class Configuration : IConfigurationDatabase
    {
        public readonly Regulus.RelationalTables.Database Database;
        public readonly Configs.BuffInfo[] BuffInfos;
        public readonly Configs.Localization[] Localizations;

        public Configuration(Regulus.RelationalTables.Database database)
        {
            Database = database;
            //BuffInfos = Database.Query<BuffInfo>().ToArray();
            Localizations = Database.Query<Configs.Localization>().ToArray();
        }

        public IEnumerable<T> Query<T>()
        {
            return Database.Query<T>();
        }
    }
}
