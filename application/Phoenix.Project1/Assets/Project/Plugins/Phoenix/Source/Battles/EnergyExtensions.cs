using System.Linq;
using System.Collections.Generic;

namespace Phoenix.Project1.Battles
{
}

namespace Phoenix.Project1.Battles.Extensions
{


    public static class ValueExtensions
    {
        
        /*public static int ToNormalize(this float val)
        {
            return (int)(Value.One * val);
        }*/
    }
    public static class EnergyExtensions
    {
        public static IEnumerable<Energy> GetEnergys(this Configs.Skill skill , Actor caster, Stage stage, Scope scope)
        {
            return skill._DamageHp(caster, stage, scope);
        }

        static IEnumerable<Energy> _DamageHp(this Configs.Skill skill, Actor caster, Stage stage, Scope scope)
        {
            foreach (var actor in scope.Unbeneficials)
            {
                
                yield return _DamageHp(skill, caster, actor);
            }
        }

        private static Energy _DamageHp(Configs.Skill skill, Actor caster, Actor target)
        {
            var casterAttack = caster.GetAttrbute(Common.Battles.AttributeType.Attack);
            var effect101 = skill.GetEffect(101);
            var effect102 = skill.GetEffect(102);
            var buff30203 = caster.GetBuff(Common.Battles.BuffType.Buff30203);
            var targetDef = target.GetAttrbute(Common.Battles.AttributeType.Defense);
            var formula = new DamageFormula(casterAttack, effect101, effect102, buff30203, targetDef , 1.05f , 1f); 
            return new Energy() { Actor = target.Id, Critical = false, Type = Common.Battles.EnergyType.Hp, Value = -formula.Damage };
           
        }
    }
}



