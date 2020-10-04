using Regulus.Remote.Client.Tcp;
using Regulus.Utility;

namespace Phoenix.Project1.Client
{
    internal class TcpUpdater : IStatus
    {
        private IOnlineable onlineable;

        public TcpUpdater(IOnlineable onlineable)
        {
            this.onlineable = onlineable;
        }

        void IStatus.Enter()
        {
            
        }

        void IStatus.Leave()
        {
 
        }

        void IStatus.Update()
        {
 
        }
    }
}