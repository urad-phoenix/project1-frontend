using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Phoenix.Project1.Client.Utilities.RxExtensions
{
    [DisallowMultipleComponent]
    public class ObserverParticleStopped : ObservableTriggerBase
    {
        public string Key;
        
        private Subject<ObserverParticleStopped> onParticleStopped;
        
        public void OnPaticleSystemStopped()
        {
            if (onParticleStopped != null) 
                onParticleStopped.OnNext(this);
        }

        public IObservable<ObserverParticleStopped> OnParticleStoppedAsObservable(string poolKey)
        {
            Key = poolKey;
            return onParticleStopped ?? (onParticleStopped = new Subject<ObserverParticleStopped>());
        }

        private void OnDisable()
        {
            if (onParticleStopped != null) 
                onParticleStopped.OnNext(this);
        }

        protected override void RaiseOnCompletedOnDestroy()
        {          
            if (onParticleStopped != null)
            {
                onParticleStopped.OnCompleted();
            }
        }
    }

    public static class RxParticleExtensions
    {
        public static IObservable<ObserverParticleStopped> OnParticleStoppedAsObserver(this GameObject gameObject, string poolKey)
        {
            if (gameObject == null) return Observable.Empty<ObserverParticleStopped>();
            
            return GetOrAddComponent<ObserverParticleStopped>(gameObject).OnParticleStoppedAsObservable(poolKey);
        }
        
        static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
            var particle = gameObject.GetComponent<ParticleSystem>();

            var main = particle.main;
            
            main.stopAction = ParticleSystemStopAction.Callback;
            
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}