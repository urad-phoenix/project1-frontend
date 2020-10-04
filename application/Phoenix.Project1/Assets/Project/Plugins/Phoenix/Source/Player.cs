using Phoenix.Project1.Common;
using Phoenix.Project1.Common.Users;
using Regulus.Remote;
using System;

namespace Phoenix.Project1.Users
{
    public class Player : IPlayer
    {
        public readonly Guid Id;
        public readonly string Account;
        readonly Property<string> _Message;
        Property<Guid> IActor.Id => new Property<Guid>(Id);

        Property<string> IActor.Account => new Property<string>(Account);

        Property<string> IActor.Message => _Message;

        public Player(string account)
        {
            Account = account;
            Id = Guid.NewGuid();
            _Message = new Property<string>();
        }

        void IPlayer.SetMessage(string message)
        {
            _Message.Value = message;
        }
    }
}
