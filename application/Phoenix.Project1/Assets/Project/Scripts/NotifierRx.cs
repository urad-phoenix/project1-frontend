using System;
using System.Collections;
using UniRx;

using Regulus.Remote.Ghost;
using UnityEngine;

namespace Phoenix.Project1.Client
{
    public static class Reactive
    {
        public static IObservable<T> FromActionPattern<T>(Action<Action<T>> addHandler, Action<Action<T>> removeHandler)
        {
            return Observable.FromEvent<System.Action<T>, T>(h => (gpi) => h(gpi), addHandler, removeHandler);
        }
    }
    public static class NotifierRx 
    {        

        public static IObservable<Regulus.Remote.INotifierQueryable> ToObservable()
        {
            return UniRx.Observable.FromCoroutine<Regulus.Remote.INotifierQueryable>(_RunWaitAgent);
        }
        private static IEnumerator _RunWaitAgent(IObserver<Regulus.Remote.INotifierQueryable> observer)
        {
            
            while (Agent.Instance == null)
            {
                yield return new WaitForEndOfFrame();           
            }
            observer.OnNext(Agent.Instance.Queryable);
            observer.OnCompleted();
        }

        public static IObservable<TGpi> Get<TGpi>(this IObservable<Regulus.Remote.INotifierQueryable> observable)
        {
            return observable.ContinueWith(_Get<TGpi>);
        }
        private static IObservable<TGpi> _Get<TGpi>(Regulus.Remote.INotifierQueryable agent)
        {
            return agent.QueryNotifier<TGpi>().Ghosts.ToObservable();
        }

        public static IObservable<TGpi> Supply<TGpi>(this IObservable<Regulus.Remote.INotifierQueryable> observable)
        {
            return observable.ContinueWith(_Supply<TGpi>);
        }

        private static IObservable<TGpi> _Supply<TGpi>(Regulus.Remote.INotifierQueryable agent)
        {
            return Observable.FromEvent<Action<TGpi>, TGpi>(h => (gpi) => h(gpi), h => agent.QueryNotifier<TGpi>().Supply += h, h => agent.QueryNotifier<TGpi>().Supply -= h);
        }

        public static IObservable<TGpi> Unsupply<TGpi>(this IObservable<Regulus.Remote.INotifierQueryable> observable)
        {
            return observable.ContinueWith<Regulus.Remote.INotifierQueryable, TGpi>(_Unsupply<TGpi>);
        }

        private static IObservable<TGpi> _Unsupply<TGpi>(Regulus.Remote.INotifierQueryable agent)
        {
            return Observable.FromEvent<System.Action<TGpi>, TGpi>(h => (gpi) => h(gpi), h => agent.QueryNotifier<TGpi>().Unsupply += h, h => agent.QueryNotifier<TGpi>().Unsupply -= h);
        }

        
    }
}
