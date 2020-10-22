

using Regulus.Utility.WindowConsoleStand;
using Regulus.Utility;
using System.Linq;

namespace Phoenix.Project1.Client
{
    internal class TcpStatus : IStatus 
    {
        private readonly IAgentProvider _Provider;
        private readonly CommandConsole _Console;
        public Regulus.Remote.INotifierQueryable Queryable;

        public TcpStatus(Regulus.Remote.IProtocol protocol, Regulus.Utility.Console.IInput input, Regulus.Utility.Console.IViewer viewer)
        {
            _Provider = new TcpAgentProvider(protocol);
            _Console = new Phoenix.Project1.Client.CommandConsole(protocol, _Provider, input, viewer);
            _Console.Launch();
            var ret = _Console.Command.Run("create-agent", new string[0]).Single() as Regulus.Remote.Value<Regulus.Remote.INotifierQueryable>;
            Queryable = ret.GetValue();            
        }

        void IStatus.Enter()
        {
            
        }

        void IStatus.Leave()
        {
            _Console.Shutdown();

        }

        void IStatus.Update()
        {
            _Console.Update();

        }
    }
}
