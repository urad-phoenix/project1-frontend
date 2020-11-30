using System;
using System.Collections.Generic;

namespace Phoenix.Playables
{
    public class BindingStrategiesFactory
    {
        static Dictionary<BindingCategory, IBindingStrategies> m_Bindings = new Dictionary<BindingCategory, IBindingStrategies>();
        
        public static IBindingStrategies QueryBinding(BindingCategory key)
        {            
            if (m_Bindings.ContainsKey(key))
            {
                return m_Bindings[key];
            }
           /* else
            {
                var attributes = ReflectionUtility.GetCustomAttributes<BindingAttribute>(true);

                var attribute = attributes.First(x => x.KeyType == key);
                
                if (attribute != null)
                {
                    var strategyAttributes = ReflectionUtility.GetCustomAttributes<BindingStrategyAttribute>(true);

                    var strategyAttribute = strategyAttributes.First(x => x.StrategyInterfaceType == attribute.BindingStrategyType);

                    if (strategyAttribute != null)
                    {
                        var bindingStrategy = Activator.CreateInstance(strategyAttribute.BindingType) as IBindingStrategies;

                        if (bindingStrategy != null)
                        {
                            m_Bindings.Add(key, bindingStrategy);
                            return bindingStrategy;
                        }    
                    }                       
                }                
            }*/

            return null;
        }

        public static void AddBindingStrategy(BindingCategory key, IBindingStrategies bindingStrategy)
        {            
            if(m_Bindings.ContainsKey(key))
                throw new Exception("BindingStrategiesFactory Is already have key " + key.ToString());
            
            m_Bindings.Add(key, bindingStrategy);
        }

        public static void RemoveAllBindings()
        {            
            m_Bindings.Clear();
        }        
    }

}