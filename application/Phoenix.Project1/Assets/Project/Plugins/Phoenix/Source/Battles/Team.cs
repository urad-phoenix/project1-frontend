using System;
using System.Linq;

namespace Phoenix.Project1.Battles
{
    public class Team
    {
        public readonly Actor[] Actors;
        public Team(params Actor[] actors)
        {
            this.Actors = actors;
        }

        public bool IsMember(Actor actor)
        {
            return Actors.Any(a => a == actor);
        }

        internal System.Collections.Generic.IEnumerable<Actor> GetDamageables()
        {
            foreach (var actor in Actors)
            {
                if(actor.IsDamageable())
                {
                    yield return actor;
                }
            }
        }

        internal Actor FindActorByLocation(int location)
        {
            return Actors.SingleOrDefault(a => a.Location == location);
        }
    }
}
