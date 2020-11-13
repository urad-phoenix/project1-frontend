namespace Phoenix.Playables
{
    using Attribute;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Reflection;
    
    public class DirectorBuilderFactory
    {
        static Dictionary<object, IPlayableBuilder> m_PlayableBuilders = new Dictionary<object, IPlayableBuilder>();
        
        public static IPlayableBuilder QueryBuilder(object key)
        {
            if (m_PlayableBuilders.ContainsKey(key))
            {
                return m_PlayableBuilders[key];
            }
            else
            {
                var attributes = ReflectionUtility.GetCustomAttributes<PlayableActionAttribute>(true);

                if (attributes.Length == 0)
                    return null;
                
                var attribute = attributes.First(x => x.Key.Equals(key));

                if (attribute != null)
                {
                    var converter = Activator.CreateInstance(attribute.ConverterType) as IPlayableBuilder;

                    if (converter != null)
                    {
                        m_PlayableBuilders.Add(key, converter);
                        return converter;
                    }    
                }
            }

            return null;
        }

        public static void AddBuilder<T>(object key, T directorBuilders) where T : IPlayableBuilder
        {         
            if(m_PlayableBuilders.ContainsKey(key))
                throw new Exception("DirectorBuilderFactory Is already have key " + key.ToString());
            
            m_PlayableBuilders.Add(key, directorBuilders);
        }

        public static void RemoveAllConverter()
        {
            foreach (var converter in m_PlayableBuilders)
            {
                converter.Value.Dispose();
            }
            m_PlayableBuilders.Clear();
        }        
    }
}