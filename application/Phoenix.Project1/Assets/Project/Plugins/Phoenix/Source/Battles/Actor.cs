using Phoenix.Project1.Common.Battles;
using Phoenix.Project1.Configs;
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    class EnergyValue
    {
        
        public readonly Property<float> Max;
        public readonly Property<float> Now;
        

        public EnergyValue(float val)
        {
            Max = new Property<float>(val);
            Now = new Property<float>(val);
        }

        internal void Diff(float value)
        {
            Now.Value += value;
            if (Now.Value > Max.Value)
                Now.Value = Max.Value;
            if (Now.Value < 0)
                Now.Value = 0;
        }
    }
    public class Actor : IActor
    {

        public readonly Configs.Actor Prototype;
        readonly System.Collections.Generic.Dictionary<MotionType, Motion> _Motions;

        readonly Project1.Battles.Spell[] _Spells;


        readonly System.Collections.Generic.Dictionary<AttributeType, Property<float>> _Attributes;
        readonly System.Collections.Generic.Dictionary<EnergyType, EnergyValue> _Energys;
        readonly System.Collections.Generic.Dictionary<BuffType, int> _Buffs;

        public Actor(Configs.Actor prototype,int instance_id, int location)
        {
            Prototype = prototype;
            _Motions = new System.Collections.Generic.Dictionary<MotionType, Motion>();
            _Motions.Add(MotionType.Entrance, new Motion { TotalFrame = 60 });
            _Motions.Add(MotionType.Forward, Prototype.Motions.Forward);
            _Motions.Add(MotionType.Back, Prototype.Motions.Back);
            _Motions.Add(MotionType.Cast1, Prototype.Motions.Cast1);
            Id = new Property<int>(instance_id);
            Location = new Property<int>(location);            
            _AvatarId = new Property<string>("hero-1");

            _Spells = _Create(Prototype).ToArray();
            _Attributes = _BuildAttributes(prototype);
            _Energys = _BuildEnergys(prototype);
            _Buffs = _BuildBuffs(prototype);
        }

        private Dictionary<BuffType, int> _BuildBuffs(Configs.Actor prototype)
        {
            var buffs = new Dictionary<BuffType, int>();
            
            return buffs;
        }

        private Dictionary<EnergyType, EnergyValue> _BuildEnergys(Configs.Actor prototype)
        {

            var energys = new Dictionary<EnergyType, EnergyValue>();
            energys.Add(EnergyType.Hp, new EnergyValue(prototype.Hp)  );
            return energys ;
        }

        private static System.Collections.Generic.Dictionary<AttributeType, Property<float>> _BuildAttributes(Configs.Actor prototype)
        {
            var attributes = new System.Collections.Generic.Dictionary<AttributeType, Property<float>>();
            attributes.Add(AttributeType.Attack, new Property<float>( prototype.Attack) );
            attributes.Add(AttributeType.Avoid, new Property<float>(prototype.Evasion));
            attributes.Add(AttributeType.CriticalDamage, new Property<float>(prototype.CriticalDamage));
            attributes.Add(AttributeType.DamageReduction, new Property<float>(0));
            attributes.Add(AttributeType.Defense, new Property<float>(prototype.Defense));
            attributes.Add(AttributeType.Hit, new Property<float>(prototype.Hit));
            attributes.Add(AttributeType.IncreasedDamage, new Property<float>(1));

            return attributes;
        }


        internal float GetBuff(Common.Battles.BuffType type)
        {
            // todo throw new NotImplementedException();
            return 0;
        }

        internal float GetAttrbute(AttributeType attack)
        {
            
            return _Attributes[attack];
        }

        internal bool IsSurvival()
        {
            return _Energys[EnergyType.Hp].Now > 0;
        }

        private System.Collections.Generic.IEnumerable<Spell> _Create(Configs.Actor actor)
        {
            foreach (var skill in actor.Skills)
            {
                yield return new Spell(skill);
            }
        }

        internal void DiffBuff(BuffType type, int count)
        {
            //todo throw new NotImplementedException();
        }

        public readonly Property<int> Id;
        Property<int> IActor.InstanceId => Id;

        internal void DiffEnergy(EnergyType type, float value)
        {
            _Energys[type].Diff(value);
        }

        public readonly Property<int> Location;        

        Property<int> IActor.Location => Location;

        internal void DiffAttribute(AttributeType type, int value)
        {
            _Attributes[type].Value += value;
        }

        Property<float> IActor.Hp => _Energys[EnergyType.Hp].Now;
        Property<float> IActor.MaxHp => _Energys[EnergyType.Hp].Max;

        Property<string> _AvatarId ;
        Property<string> IActor.AvatarId => _AvatarId;

        internal bool IsMovable()
        {
            return _Energys[EnergyType.Hp].Now > 0;
        }

        public Project1.Battles.Spell CastSpell()
        {
            return _Spells[0];
        }

        public Motion GetMotion(MotionType type)
        {
            return _Motions[type];
        }

      
    }
}



