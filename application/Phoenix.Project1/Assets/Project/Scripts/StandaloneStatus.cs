using Regulus.Remote;
using Regulus.Utility;
using Regulus.Utility.WindowConsoleStand;
using System.Linq;

namespace Phoenix.Project1.Client
{
    internal class StandaloneStatus : IServiceStatus
    {

        readonly Users.IUserEntry _Entry;
        private readonly Console.IViewer _Viewer;
        readonly Phoenix.Project1.Client.CommandConsole _Console;
        readonly StabdaloneAgentProvider _Provider;
        private  System.IDisposable _Runner;
        INotifierQueryable _Queryable;
        public StandaloneStatus(Regulus.Remote.IProtocol protocol, Game.IConfigurationDatabase resource, Console.IInput input, Console.IViewer viewer)
        {
            Common.ILobby lobby = new Phoenix.Project1.Users.Lobby();
            var entry = new Phoenix.Project1.Users.Entry(lobby, resource);
            _Provider = new StabdaloneAgentProvider(protocol, entry);
            _Console = new CommandConsole(protocol, _Provider,input , viewer);
            
            _Entry = entry;
            _Viewer = viewer;
            }

        INotifierQueryable IServiceStatus.Queryable => _Queryable;

        void IStatus.Enter()
        {
            _Console.Launch();
            var ret = _Console.Command.Run("create-agent", new string[0]).Single() as Regulus.Remote.Value<INotifierQueryable>;
            _Queryable = ret.GetValue();

            _Runner = new Scripts.Runner(_Queryable, new Scripts.ScriptsCommander("s-{0}", _Console.Command), _Viewer);

        }

        void IStatus.Leave()
        {
            _Runner.Dispose();
            _Entry.Dispose();
            _Console.Shutdown();
        }

        void IStatus.Update()
        {
            _Console.Update();        
        }
    }
}
