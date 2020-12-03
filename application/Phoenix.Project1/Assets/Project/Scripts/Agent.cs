using Phoenix.Project1.Client.UI;
using Regulus.Remote;
using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Phoenix.Project1.Addressable;
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
        public bool Standalone;
        public Phoenix.Project1.Client.UI.Console Console;
        readonly  Regulus.Utility.StatusMachine _Machine;
        private  IProtocol _Protocol;
        IServiceStatus _Status;
        public Regulus.Remote.INotifierQueryable Queryable => _Status.Queryable;
        

        public Agent()
        {
            _Machine = new Regulus.Utility.StatusMachine();
            var type = Regulus.Remote.Protocol.ProtocolProvider.GetProtocols().Single();
            _Protocol = System.Activator.CreateInstance(type) as Regulus.Remote.IProtocol;            

            
            _ToReady();
        }

        
        private IObservable<bool> _SetStandalone(Phoenix.Project1.Game.Configuration resource)
        {

            IServiceStatus status = new StandaloneStatus(_Protocol, resource, Console, Console);
            _Status = status;
            _Machine.Push(status); 
            return UniRx.Observable.Return(true);
        }
       
        private void Start()
        {
            if (Standalone)
            {
                _SetStandalone().Subscribe().AddTo(gameObject);

            }
            else
                _SetTcp().Subscribe();
        }

        private void Update()
        {
            _Machine.Update();
        }
        private void OnDestroy()
        {
            _Machine.Termination();
        }

        System.IObservable<bool> _SetStandalone()
        {
            return
                from config in Configuration.ToObservable()
                from result in _SetStandalone(config.Resource)
                select result;
        }

        System.IObservable<bool> _SetTcp()
        {
            IServiceStatus status = new TcpStatus(_Protocol, Console , Console);
            _Status = status;
            _Machine.Push(status);
            return UniRx.Observable.Return(true);
        }       

        private void _ToReady()
        {
            IServiceStatus status = new IdleStatus();
            _Status = status;
            _Machine.Push(status);            
        }
    }
}
