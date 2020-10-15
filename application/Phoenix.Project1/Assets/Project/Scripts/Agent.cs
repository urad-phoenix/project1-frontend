using Phoenix.Project1.Client.UI;
using Regulus.Remote;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

        

        public static IObservable<Agent> ToObservable()
        {
            return UniRx.Observable.FromCoroutine<Agent>(_RunWaitAgent);
        }
        private static IEnumerator _RunWaitAgent(IObserver<Agent> observer)
        {

            while (Agent.Instance == null)
            {
                yield return new WaitForEndOfFrame();
            }
            observer.OnNext(Agent.Instance);
            observer.OnCompleted();
        }

        readonly  Regulus.Utility.StatusMachine _Machine;
        private  IProtocol _Protocol;
        private  readonly Regulus.Remote.Ghost.IAgent _Agent;
        public Regulus.Remote.INotifierQueryable Queryable => _Agent;
        public bool Active => _Machine.Current != null;

        internal void SetEmpty()
        {
            _ToReady();
        }

        public Agent()
        {
            _Machine = new Regulus.Utility.StatusMachine();
            var type = Regulus.Remote.Protocol.ProtocolProvider.GetProtocols().Single();
            _Protocol = System.Activator.CreateInstance(type) as Regulus.Remote.IProtocol;
            _Agent = Regulus.Remote.Client.Provider.CreateAgent(_Protocol);
            _ToReady();
        }

        
        private IObservable<bool> _SetStandalone(Phoenix.Project1.Game.Configuration resource)
        {            
            Common.ILobby lobby = new Phoenix.Project1.Users.Lobby();
            var entry = new Phoenix.Project1.Users.Entry(lobby, localization.GetResource());
            var service = Regulus.Remote.Standalone.Provider.CreateService(_Protocol, entry);
            _Machine.Push(new StandaloneStatus(service, _Agent));       
            return UniRx.Observable.Return(true);
        }

        private void Update()
        {
            _Machine.Update();
        }
        private void OnDestroy()
        {
            _Machine.Termination();
        }

        internal System.IObservable<bool> SetStandalone()
        {
            return
                from config in Configuration.ToObservable()
                from result in _SetStandalone(config.Resource)
                select result;
        }

        internal System.IObservable<bool> SetTcp(IPAddress ip_address, int port)
        {
            var status = new TcpStatus(ip_address, port, _Agent);
            status.ErrorEvent += _TcpError;
            _Machine.Push(status);
            return status.Result;
        }

        private void _TcpError(SocketError error)
        {
            var box = MessageBoxProvider.Instance.Open($"與Server斷線:{error}", "","ok");
            box.Buttons[0].OnClickAsObservable().Subscribe(_ => MessageBoxProvider.Instance.Close(box));
            _ToReady();
        }

        private void _ToReady()
        {

            _Machine.Termination();
        }
    }
}
