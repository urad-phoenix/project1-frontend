namespace Phoenix.Playables.Attribute
{
    using System;
    
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingStrategyAttribute :   Attribute
    {
        public readonly Type BindingType;

        public readonly Type StrategyInterfaceType;
        
        public BindingStrategyAttribute(Type bindingType, Type strategyInterfaceType)
        {
            BindingType = bindingType;
            StrategyInterfaceType = strategyInterfaceType;
        }
    }
}