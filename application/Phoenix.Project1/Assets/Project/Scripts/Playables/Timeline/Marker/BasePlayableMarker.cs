using Phoenix.Playables.Attribute;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables.Markers
{
    public abstract class BasePlayableMarker : Marker, INotificationOptionProvider
    {
        [NotificationBinding]
        public string Name;

        public NotificationFlags Flags;
        
        public PropertyName id
        {
            get { return Name; }
        }
        public NotificationFlags flags
        {
            get { return Flags; }
        }
    }
}