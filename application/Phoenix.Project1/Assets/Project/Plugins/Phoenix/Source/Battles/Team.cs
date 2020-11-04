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
    }
}
