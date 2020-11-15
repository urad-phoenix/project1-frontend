using Phoenix.Project1.Common.Battles;

namespace Phoenix.Project1.Client.Battles
{
    public class Role : Unit
    {
        public int Location;

        public Avatar GetAvatar()
        {
            return GetComponentInChildren<Avatar>();
        }
    }
}