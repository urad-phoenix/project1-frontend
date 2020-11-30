using Regulus.Remote;
using System;
using System.Net;

namespace Phoenix.Project1.Client.Scripts
{
    public class Runner : IDisposable
    {
        readonly ScriptsCommander _ScriptsCommander;
        readonly LoginScript _LoginScript;
        readonly BattleScript _BattleScript;
        public Runner(INotifierQueryable notifier, ScriptsCommander command , Regulus.Utility.Console.IViewer viewer)
        {
            _LoginScript = new LoginScript(notifier, _DefaultIp(), _DefaultAccount());
            _BattleScript = new BattleScript(notifier, viewer);
            _ScriptsCommander = command;


            _ScriptsCommander.Add("tcp-login", _LoginScript);
            _ScriptsCommander.Add("battle1", _BattleScript);
        }

        private string _DefaultAccount()
        {
            return "jc-bot";
        }

        private IPEndPoint _DefaultIp()
        {
            return new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11366);
        }

        void IDisposable.Dispose()
        {
            _ScriptsCommander.Remove("battle1");
            _ScriptsCommander.Remove("tcp-login");
        }
    }


}
