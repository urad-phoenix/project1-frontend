namespace Phoenix.Project1.Client.Battles
{
    public class TimelienBehaviour : IStateBehaviour
    {
        public static TimelienBehaviour Create(BindingHandle handle)
        {
            var behaviour = new TimelienBehaviour();
            
            behaviour.SetHandle(handle);
            
            return behaviour;
        }

        public void Star(StateBinding binding)
        {
            throw new System.NotImplementedException();
        }

        public void Stop(StateBinding binding)
        {
            throw new System.NotImplementedException();
        }

        public void Update(StateBinding binding)
        {
            throw new System.NotImplementedException();
        }

        public void SetHandle(BindingHandle bindingHandle)
        {
            _BindingHandle = bindingHandle;
        }

        private BindingHandle _BindingHandle;
    }
}