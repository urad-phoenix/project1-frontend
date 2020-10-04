using Phoenix.Project1.Common;
using Regulus.Remote;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Project1.Users
{
    public class Entry : Regulus.Remote.IEntry
    {
        readonly List<User> _Users;
        private readonly ILobby _Lobby;

        public Entry(ILobby lobby)
        {
            _Users = new List<User>();
            this._Lobby = lobby;
        }
        void IBinderProvider.AssignBinder(IBinder binder, object state)
        {
            
            User user = new User(binder, _Lobby); 
            _Users.Add(user);

            binder.BreakEvent += () => {
                _Users.Remove(user);
            };
        }
    }
}
