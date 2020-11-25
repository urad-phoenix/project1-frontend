using System.Collections.Generic;

namespace Phoenix.Project1.Battles.Decisions
{
    struct DamageHpDecisioner
    {
        float _CasterAttack;
        float _SkillEffect101;
        float _SkillEffect102;

    }
}
namespace Phoenix.Project1.Battles.Extensions
{
    
    public static class DecisionExtensions
    { 
        /*public static float DamageHp(this Configs.Skill skill , Actor caster, Actor target)
        {
            
        }*/
    }
    public static class AttuributeExtensions
    {
        public static IEnumerable<Attribute> GetAttributes(this Configs.Skill skill, Actor caster, Stage stage, Scope scope)
        {
            return new Attribute[0];
        }
    }
}

