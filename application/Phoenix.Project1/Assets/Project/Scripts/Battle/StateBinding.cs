namespace Phoenix.Project1.Client.Battles
{
    public class StateBinding : IStateBinding
    {
        private BindingHandle _Handle;                
        
        static readonly StateBinding m_NullPlayable = new StateBinding(BindingHandle.Null);
        
        public static StateBinding Null { get { return m_NullPlayable; } }
        
        public StateBinding(BindingHandle handle)
        {
            _Handle = handle;
        }

        public BindingHandle GetHandle()
        {
            return _Handle;
        }
             
    }
}