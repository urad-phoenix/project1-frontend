using Regulus.Remote;
using Regulus.Remote.Ghost;
using System.Linq;
using System.Net;
using UniRx;
using UnityEngine;
namespace Phoenix.Project1.Client
{
    public class Agent : MonoBehaviour
    {
        public  static Agent Instance => _GetInstance();
        
        private static Agent _GetInstance()
        {
            return UnityEngine.GameObject.FindObjectOfType<Phoenix.Project1.Client.Agent>();
        }

        readonly  Regulus.Utility.StatusMachine _Machine;
        private  IProtocol _Protocol;
        private  Regulus.Remote.Ghost.IAgent _Agent;
        public Regulus.Remote.INotifierQueryable Queryable => _Agent;        

        public Agent()
        {
            _Machine = new Regulus.Utility.StatusMachine();
            var type = Regulus.Remote.Protocol.ProtocolProvider.GetProtocols().Single();
            _Protocol = System.Activator.CreateInstance(type) as Regulus.Remote.IProtocol;
            _Agent = Regulus.Remote.Client.Provider.CreateAgent(_Protocol);
            
        }

        
        private void _SetStandalone()
        {            
            Common.ILobby lobby = new Phoenix.Project1.Users.Lobby();
            var entry = new Phoenix.Project1.Users.Entry(lobby);
            var service = Regulus.Remote.Standalone.Provider.CreateService(_Protocol, entry);            
            _Machine.Push(new StandaloneUpdater(service, _Agent));            
        }

        private void Update()
        {
            _Machine.Update();
        }
        private void OnDestroy()
        {
            _Machine.Termination();
        }

        internal System.IObservable<Unit> SetStandalone()
        {
            _SetStandalone();
            return UniRx.Observable.Return(Unit.Default);

        }
    }
}
