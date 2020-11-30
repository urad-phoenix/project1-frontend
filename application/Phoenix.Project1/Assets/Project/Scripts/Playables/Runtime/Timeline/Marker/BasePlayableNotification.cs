using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables.Markers
{
    public abstract class BasePlayableNotification : INotification
    {       
        public abstract PropertyName id { get; }
    }
}