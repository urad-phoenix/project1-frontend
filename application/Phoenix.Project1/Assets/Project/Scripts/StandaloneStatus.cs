using Regulus.Remote;
using Regulus.Utility;
using Regulus.Utility.WindowConsoleStand;
using System.Linq;

namespace Phoenix.Project1.Client
{
    internal class StandaloneStatus : IStatus
    {

        readonly Users.IUserEntry _Entry;        
        readonly Phoenix.Project1.Client.CommandConsole _Console;
        readonly StabdaloneAgentProvider _Provider;
        
        public INotifierQueryable Queryable;
        public StandaloneStatus(Regulus.Remote.IProtocol protocol, Game.IConfigurationDatabase resource, Console.IInput input, Console.IViewer viewer)
        {
            Common.ILobby lobby = new Phoenix.Project1.Users.Lobby();
            var entry = new Phoenix.Project1.Users.Entry(lobby, resource);
            _Provider = new StabdaloneAgentProvider(protocol, entry);
            _Console = new CommandConsole(protocol, _Provider,input , viewer);
            _Console.Launch();
            var ret = _Console.Command.Run("create-agent",new string[0]).Single() as Regulus.Remote.Value<INotifierQueryable>;
            Queryable = ret.GetValue();            
            _Entry = entry;            
        }

        void IStatus.Enter()
        {
            
        



        }

        void IStatus.Leave()
        {
        
            _Entry.Dispose();
            _Console.Shutdown();

        }

        void IStatus.Update()
        {
            _Console.Update();
        
        }
    }
}
