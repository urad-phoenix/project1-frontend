using System;

namespace Phoenix.Playables.Attribute
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TrackMarkerBindingAttribute : System.Attribute
    {
        public Type TrackType;

        public TrackMarkerBindingAttribute(Type type)
        {
            TrackType = type;
        }
    }
}