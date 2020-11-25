using Phoenix.Project1.Common.Battles;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    public class SpellCast
    {
        public  readonly int Location;
        private readonly IEnumerable<Attribute> _Attributes;
        private readonly IEnumerable<Energy> _Energys;
        private readonly IEnumerable<Buff> _Buffs;

        

        public SpellCast(int location, IEnumerable<Attribute> attributes, IEnumerable<Energy> energys, IEnumerable<Buff> buffs)
        {
            this.Location = location;
            this._Attributes = attributes;
            this._Energys = energys;
            this._Buffs = buffs;
        }

        internal IEnumerable<Effect> ToEffects()
        {
            return _Damage().Union(_DamageCritical() ) ;
        }

        private IEnumerable<Effect> _DamageCritical()
        {
            foreach (var energy in _Energys)
            {
                if (!energy.Critical)
                    continue;

                yield return new Effect() { Actor = energy.Actor, Type = EffectType.InjureCritical};
            }
        }

        private IEnumerable<Effect> _Damage()
        {
            foreach (var energy in _Energys)
            {
                if (energy.Critical)
                    continue;

                yield return new Effect() { Actor = energy.Actor, Type = EffectType.Injure , Value = (int)(0-energy.Value)};
            }
        }

        internal void Occurrence(Stage stage)
        {
            _Occurrence(_Attributes , stage);
            _Occurrence(_Energys, stage);
            _Occurrence(_Buffs, stage);
        }

        private void _Occurrence(IEnumerable<Buff> buffs, Stage stage)
        {
            foreach (var buff in buffs)
            {
                var actor = stage.GetActor(buff.Actor);
                actor.DiffBuff(buff.Type, buff.Count);
            }
        }

        private void _Occurrence(IEnumerable<Energy> energys, Stage stage)
        {
            foreach (var energy in energys)
            {
                var actor = stage.GetActor(energy.Actor);
                actor.DiffEnergy(energy.Type, energy.Value);
            }
        }

        private void _Occurrence(IEnumerable<Attribute> attributes, Stage stage)
        {
            foreach (var attr in attributes)
            {
                var actor = stage.GetActor(attr.Actor);
                actor.DiffAttribute(attr.Type, attr.Value);
            }
        }
    }
}