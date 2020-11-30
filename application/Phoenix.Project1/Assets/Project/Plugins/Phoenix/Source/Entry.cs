using Phoenix.Project1.Common;
using Phoenix.Project1.Game;
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Project1.Users
{
  
    public class Entry : IUserEntry
    {
        readonly IConfigurationDatabase _Configs;

        
        private readonly ILobby _Lobby;
        readonly Regulus.Utility.Updater _Updater;
        readonly System.Threading.Tasks.Task _Runner;
        bool _RunnerEnable;
        // for standalone
        public Entry() : this(new Lobby() ,new FakeConfiguration() )
        {

        }
        // for remote
        public Entry(ILobby lobby, IConfigurationDatabase resource)
        {
            _Configs = resource;

            this._Lobby = lobby;
            _Updater = new Regulus.Utility.Updater();
            _RunnerEnable = true;
            _Runner = new System.Threading.Tasks.Task(_Run , System.Threading.Tasks.TaskCreationOptions.LongRunning );
            _Runner.Start();
        }

        private void _Run()
        {
            var ar = new Regulus.Utility.AutoPowerRegulator(new Regulus.Utility.PowerRegulator());
            while(_RunnerEnable)
            {
                _Updater.Working();
                ar.Operate();
            }

            _Updater.Shutdown();
        }

        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {

            var user = new User(binder, _Lobby, _Configs);
            _Updater.Add(user);



        }

        void IDisposable.Dispose()
        {
            
            _RunnerEnable = false;
            _Runner.Wait();
        }
    }
}
