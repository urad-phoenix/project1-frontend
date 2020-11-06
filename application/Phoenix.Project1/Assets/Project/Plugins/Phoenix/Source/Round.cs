using System.Collections.Generic;

namespace Phoenix.Project1.Game.Battles
{
    public class Round
    {
        public int Sequence;
        public List<Action> Actions;
        public Round(int sequence)
        {
            Sequence = sequence;
            Actions = new List<Action>();
        }
    }
}