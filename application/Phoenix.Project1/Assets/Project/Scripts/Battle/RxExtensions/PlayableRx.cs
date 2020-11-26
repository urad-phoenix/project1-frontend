using System;
using Phoenix.Project1.Client.Utilities.RxExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Project1.Client.Battles
{
    public static class PlayableRx
    {
        public static IObservable<PlayableDirector> PlayAsObservable(this PlayableDirector director, IDisposable disposable)
        {
            return Observable.Create<PlayableDirector>(obs =>
            {                
                director.Play();
                
                return Observable.EveryUpdate().Subscribe(u =>
                {
                    director.stopped += playableDirector =>
                    {
                        obs.OnNext(playableDirector);
                        obs.OnCompleted();
                        disposable.Dispose();
                    };
                });
            });
        }                
    }
}