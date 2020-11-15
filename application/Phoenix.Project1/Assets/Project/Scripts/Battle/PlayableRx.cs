using System;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public static class PlayableRx
    {
        public static IObservable<PlayableDirector> PlayAsObservable(this PlayableDirector director)
        {
            director.Play();
            
            return Observable.Create<PlayableDirector>(obs =>
            {
                Observable.EveryUpdate().Subscribe(u =>
                {
                    director.stopped += playableDirector =>
                    {
                        obs.OnNext(playableDirector);
                        obs.OnCompleted();
                    };
                    
                }).AddTo(director);

                return Disposable.Empty;
            });
        }
        
        public static IObservable<ObserverParticleStopped> OnParticleStoppedAsObserver(this GameObject gameObject, string poolKey)
        {
            if (gameObject == null) return Observable.Empty<ObserverParticleStopped>();
            
            return GetOrAddComponent<ObserverParticleStopped>(gameObject).OnParticleStoppedAsObservable(poolKey);
        }
        
        static T GetOrAddComponent<T>(GameObject gameObject)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}