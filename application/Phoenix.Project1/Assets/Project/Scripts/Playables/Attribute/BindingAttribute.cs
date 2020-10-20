namespace Phoenix.Playables.Attribute
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingAttribute : System.Attribute
    {
        public readonly BindingCategory KeyType;
        
        public readonly Type BindingStrategyType;        
        
        public BindingAttribute(Type bindingStrategyType, BindingCategory keyType)
        {            
            BindingStrategyType = bindingStrategyType;
            KeyType = keyType;
        }
    }
}