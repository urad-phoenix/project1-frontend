using System.Collections.Generic;

namespace Phoenix.Project1.Battles
{
    class ActorComparer : System.Collections.Generic.IComparer<Actor>
    {
        int IComparer<Actor>.Compare(Actor x, Actor y)
        {
            return x.Id - y.Id;
        }
    }
}
