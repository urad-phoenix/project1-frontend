using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    [DisallowMultipleComponent]
    public class ObserverParticleStopped : ObservableTriggerBase
    {
        public string Key;
        
        private Subject<ObserverParticleStopped> onParticleStopped;
        
        void OnParticleSystemStopped()
        {
            if (onParticleStopped != null) onParticleStopped.OnNext(this);
        }

        public IObservable<ObserverParticleStopped> OnParticleStoppedAsObservable(string poolKey)
        {
            Key = poolKey;
            return onParticleStopped ?? (onParticleStopped = new Subject<ObserverParticleStopped>());
        }                    

        protected override void RaiseOnCompletedOnDestroy()
        {          
            if (onParticleStopped != null)
            {
                onParticleStopped.OnCompleted();
            }
        }
    }        
}