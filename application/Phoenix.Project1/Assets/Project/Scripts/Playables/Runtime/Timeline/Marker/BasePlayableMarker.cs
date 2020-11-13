using System;
using Phoenix.Playables.Attribute;
using UnityEngine;
using UnityEngine.Timeline;

namespace Phoenix.Playables.Markers
{
    [Serializable]
    public abstract class BasePlayableMarker : Marker
    {
        [NotificationBinding]
        public string Name;
        
        public PropertyName id
        {
            get { return Name; }
        }        
    }
}