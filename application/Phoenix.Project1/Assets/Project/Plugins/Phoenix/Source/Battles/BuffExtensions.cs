using System.Collections.Generic;

namespace Phoenix.Project1.Battles.Extensions
{
    public static class BuffExtensions
    {
        public static IEnumerable<Buff> GetBuffs(this Configs.Skill skill, Actor caster, Stage stage, Scope scope)
        {
            return new Buff[0];
        }
    }
}
