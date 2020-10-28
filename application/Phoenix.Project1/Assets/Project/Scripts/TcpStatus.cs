

using Regulus.Utility.WindowConsoleStand;
using Regulus.Utility;
using System.Linq;
using Regulus.Remote;
using System;

namespace Phoenix.Project1.Client
{
    internal class TcpStatus : IServiceStatus , IAgentProvider
    {
        private readonly TcpAgent _Agent;
        private readonly CommandConsole _Console;
        private readonly INotifierQueryable _Queryable;

        public TcpStatus(Regulus.Remote.IProtocol protocol, Regulus.Utility.Console.IInput input, Regulus.Utility.Console.IViewer viewer)
        {
            _Agent = new Phoenix.Project1.Client.TcpAgent(protocol);
            _Console = new Phoenix.Project1.Client.CommandConsole(protocol, this, input, viewer);
            _Queryable = _Agent;
        }
        INotifierQueryable IServiceStatus.Queryable => _Queryable;
        void IStatus.Enter()
        {
            _Console.Launch();
            _Console.Command.Run("create-agent", new string[0]).Single();
            
        }

        void IStatus.Leave()
        {
            _Console.Shutdown();
        }

        Tuple<INotifierQueryable, IUpdatable> IAgentProvider.Spawn()
        {
            return new Tuple<INotifierQueryable, IUpdatable>(_Agent, _Agent);
        }

        void IStatus.Update()
        {
            _Console.Update();
        }
    }
}
