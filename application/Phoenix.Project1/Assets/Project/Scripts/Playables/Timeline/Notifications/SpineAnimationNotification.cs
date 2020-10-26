using UnityEngine;

namespace Phoenix.Playables.Markers
{
    public class SpineAnimationNotification : BasePlayableNotification
    {
        public string Name;
        public override PropertyName id
        {
            get { return Name; }
        }
    }
}