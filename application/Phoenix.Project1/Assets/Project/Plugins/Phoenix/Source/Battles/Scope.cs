using Regulus.Remote;
using System.Collections.Generic;

namespace Phoenix.Project1.Battles
{
    public class Scope
    {
        public readonly int Location;
        public readonly IEnumerable<Actor> Beneficials;
        public readonly IEnumerable<Actor> Unbeneficials;
        

        public Scope(int location, IEnumerable<Actor> unbeneficials)
        {
            Location = location;
            Unbeneficials = unbeneficials;
            Beneficials = new Actor[0];
        }
    }
}