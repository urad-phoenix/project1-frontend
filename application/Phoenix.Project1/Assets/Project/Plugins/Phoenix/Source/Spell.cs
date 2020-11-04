using Phoenix.Project1.Common.Battles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    public class Spell
    {
        public MotionType Motion;

        public int Id { get; internal set; }

        public Spell()
        {
            Id = 1;
            Motion = MotionType.Spell1;
        }

        internal IEnumerable<Effect> CreateEffects(Actor actor,Stage stage)
        {
            if (stage.Attacker.IsMember(actor))
                return new Effect[] { new Effect { Actor = stage.Defender.Actors.Where(a => a.Hp.Value > 0).First().Id , Type = EffectType.Injure , Value = 10 } } ;

            if (stage.Defender.IsMember(actor))
                return new Effect[] { new Effect { Actor = stage.Attacker.Actors.Where(a => a.Hp.Value > 0).First().Id, Type = EffectType.Injure, Value = 10 } };

            return new Effect[0];
        }

        internal int GetLocation(Actor actor, Stage stage)
        {
            if (stage.Attacker.IsMember(actor))
                return stage.Defender.Actors.First().Location;

            if (stage.Defender.IsMember(actor))
                return stage.Attacker.Actors.First().Location;

            return actor.Location;
        }
    }
}