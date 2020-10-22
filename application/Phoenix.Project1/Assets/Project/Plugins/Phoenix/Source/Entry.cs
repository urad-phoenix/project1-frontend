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

        readonly List<IDisposable> _Users;
        private readonly ILobby _Lobby;


        // for standalone
        public Entry() : this(new Lobby() ,new FakeConfiguration() )
        {

        }
        // for remote
        public Entry(ILobby lobby, IConfigurationDatabase resource)
        {
            _Configs = resource;

            _Users = new List<IDisposable>();
            this._Lobby = lobby;
        }
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {

            IDisposable user = new User(binder, _Lobby); 
            _Users.Add(user);

            binder.BreakEvent += () => {
                user.Dispose();
                _Users.Remove(user);
            };
        }

        void IDisposable.Dispose()
        {
            foreach (var user in _Users)
            {
                user.Dispose();
            }
        }
    }
}
