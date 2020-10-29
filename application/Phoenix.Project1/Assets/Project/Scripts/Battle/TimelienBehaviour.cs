namespace Phoenix.Project1.Client.Battles
{
    public class TimelienBehaviour : IStateBehaviour
    {
        public static TimelienBehaviour Create()
        {
            var behaviour = new TimelienBehaviour();                       
            
            return behaviour;
        }

        public void Start(StateBinding binding)
        {
            var controller = binding.GetHandle().GetReferenceObject() as BattleController;
            
            //controller.GetRole()
        }

        public void Stop(StateBinding binding)
        {            
        }

        public void Update(StateBinding binding)
        {           
        }     
    }
}