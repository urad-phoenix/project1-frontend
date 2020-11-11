using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using Regulus.Utility;

namespace Phoenix.Project1.Client.Scripts
{
    public class ScriptsCommander
    {
        private readonly string _Format;
        private readonly Command _Command;

        public ScriptsCommander(string format, Command command)
        {
            this._Format = format;
            this._Command = command;
        }

        public void Add(string name, IScriptable scriptable)
        {
            var command = string.Format(_Format, name);
            _Command.Register($"{command}-start", scriptable.Start);
            _Command.Register($"{command}-end", scriptable.End);
        }

        public void Remove(string name)
        {            
            _Command.Unregister($"{name}-start");
            _Command.Unregister($"{name}-end");
        }
    }


}
