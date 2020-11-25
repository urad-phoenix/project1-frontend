using Phoenix.Project1.Battles.SkillExtensions;
using Phoenix.Project1.Common.Battles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1.Battles.SkillExtensions
{
}
namespace Phoenix.Project1.Battles
{
    public class Buff
    {
        public int Actor;
        public BuffType Type;
        public int Count;
    }

    public class Energy
    {
        public int Actor;
        public EnergyType Type;
        public float Value;
        public bool Critical;
    }

    public class Attribute
    {
        public int Actor;
        public int Value;
        public AttributeType Type;
    }

    public class Spell
    {
        public readonly MotionType Motion;
        private readonly Configs.Skill _Skill;

        public Spell(Configs.Skill skill)
        {            
            Motion = MotionType.Cast1;
            this._Skill = skill;
        }

        
        internal SpellCast CreateCast(Actor caster, Stage stage)
        {
            
            var scope = _GetScope(caster, stage);
            IEnumerable<Attribute> attributes = _GetAttrubutes(caster, stage, scope);
            IEnumerable<Buff> buffs = _GetBuffs(caster, stage, scope);
            IEnumerable<Energy> energys = _GetEnergys(caster, stage, scope);

            var cast = new SpellCast(scope.Location, attributes, energys, buffs);
            return cast;
        }

        private IEnumerable<Energy> _GetEnergys(Actor caster, Stage stage, Scope scope)
        {
            return _Skill.GetEnergys(caster , stage , scope);
        }

        private IEnumerable<Buff> _GetBuffs(Actor caster, Stage stage, Scope scope)
        {
            return _Skill.GetBuffs(caster, stage, scope);
        }

        private IEnumerable<Attribute> _GetAttrubutes(Actor caster, Stage stage, Scope scope)
        {
            return _Skill.GetAttributes(caster, stage, scope);
        }

        Scope _GetScope(Actor caster, Stage stage)
        {
            // 找出所有有利與不利的角色
            var target = _GetLocation(caster, stage);
            return new Scope(target.Location, new[] { target });
        }
        

        private System.Collections.Generic.IEnumerable<Actor> _FindActors(Actor actor, int location, Stage stage, Configs.EffectTarget effectTarget)
        {
            return stage.GetActorsByEffectTarget(actor, location, effectTarget);
        }

        private Actor _GetLocation(Actor actor, Stage stage)
        {
            return stage.GetLocation(actor, _Skill.Location);
        }
    }
}
