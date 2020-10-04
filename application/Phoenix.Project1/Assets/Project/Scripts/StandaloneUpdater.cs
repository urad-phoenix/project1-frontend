using Regulus.Utility;
namespace Phoenix.Project1.Client
{
    internal class StandaloneUpdater : IStatus
    {
        private readonly Regulus.Remote.Standalone.IService _Service;
        private readonly Regulus.Remote.Ghost.IAgent _Agent;

        public StandaloneUpdater(Regulus.Remote.Standalone.IService service, Regulus.Remote.Ghost.IAgent agent)
        {
            this._Service = service;
            _Agent = agent;
        }

        void IStatus.Enter()
        {
            _Service.Join(_Agent);
        }

        void IStatus.Leave()
        {
            _Service.Leave(_Agent);
            _Service.Dispose();
        }

        void IStatus.Update()
        {
            _Agent.Update();
        }
    }
}
