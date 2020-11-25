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

        internal System.Collections.Generic.IEnumerable<Actor> GetSurvivors()
        {
            foreach (var actor in Actors)
            {
                if(actor.IsSurvival())
                {
                    yield return actor;
                }
            }
        }
    }
}
