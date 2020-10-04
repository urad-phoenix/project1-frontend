using Phoenix.Project1.Common;
using Regulus.Remote;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Phoenix.Project1.Users
{
    public class Lobby : ILobby
    {
        readonly Notifier<IPlayer> _Players;
        INotifier<IPlayer> ILobby.Players => _Players;


        public Lobby()
        {
            _Players = new Notifier<IPlayer>();
        }
        Value<Guid> ILobby.LoadPlayer(string account)
        {
            var player = (from p in _Players where p.Account.Value == account select p).SingleOrDefault();
            if(player == null)
            {
                player = new Player(account);
                _Players.Items.Add(player);
            }
            return player.Id.Value;
        }
    }
}
