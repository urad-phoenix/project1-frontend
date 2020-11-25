using System.Collections.Generic;

namespace Phoenix.Project1.Battles.SkillExtensions
{
    public static class AttuributeExtensions
    {
        public static IEnumerable<Attribute> GetAttributes(this Configs.Skill skill, Actor caster, Stage stage, Scope scope)
        {
            return new Attribute[0];
        }
    }
}
