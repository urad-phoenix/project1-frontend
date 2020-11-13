using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables.Markers
{
    public class VFXNotification : BasePlayableNotification
    {
        public string Name;
        
        public override PropertyName id
        {
            get { return Name; }
        }
    }
}