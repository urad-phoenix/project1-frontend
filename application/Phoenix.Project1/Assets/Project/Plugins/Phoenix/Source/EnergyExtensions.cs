using System.Linq;
using System.Collections.Generic;

namespace Phoenix.Project1.Battles.SkillExtensions
{
    
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
            var effectPercent = skill.GetEffect(101);
            var effectValue = skill.GetEffect(102);

            var v1 = (casterAttack * effectPercent + effectValue);

            var targetDefense = target.GetAttrbute(Common.Battles.AttributeType.Defense);
            var buffDefenseless = caster.GetBuff(Common.Battles.BuffType.Buff30203);
            var effectExtra4 = skill.GetEffect(4);
            var v2 = (targetDefense - buffDefenseless - effectExtra4);
            var effectExtra5 = skill.GetEffect(5);
            var v3 = (casterAttack + effectExtra5);
            var v4 = targetDefense - buffDefenseless - effectExtra4;
            var v5 = casterAttack + effectExtra5 - 1.05f;
            var casterIncreasedDamage = caster.GetAttrbute(Common.Battles.AttributeType.IncreasedDamage);
            var targetDamageReduction = caster.GetAttrbute(Common.Battles.AttributeType.DamageReduction);
            var effectExtra3 = skill.GetEffect(3);
            var v6 = casterIncreasedDamage + effectExtra3 - targetDamageReduction;

           /* var casterHit = caster.GetAttrbute(Common.Battles.AttributeType.Hit);
            var targetAvoid = caster.GetAttrbute(Common.Battles.AttributeType.Avoid);
            var effectExtra2 = skill.GetEffect(2);
            var avoid = 1;// 1 - (casterHit + effectExtra2 - targetAvoid);
            var effectExtra6 = skill.GetEffect(6);
            var casterCriticalDamage = caster.GetAttrbute(Common.Battles.AttributeType.CriticalDamage);
            var critical = 1 + casterCriticalDamage + effectExtra6;// 1 - (casterHit + effectExtra2 - targetAvoid);*/


//            var v =0- (v1 * (1 - v2 / v3) / (v4 / v5) * v6);
            var v = -10;
            return new Energy()
            { Actor = target.Id , Critical = false , Type = Common.Battles.EnergyType.Hp , Value =v };
           
        }
    }
}


