namespace Phoenix.Playables.Attribute
{    
    using System;
    
    [AttributeUsage(AttributeTargets.Class)]
    public class PlayableActionAttribute : Attribute
    {
        public readonly object Key;
    
        public readonly Type ConverterType;
    
        public PlayableActionAttribute(object key, Type convertType)
        {
            Key = key;
            ConverterType = convertType;
        }
    }
}

