using System;
using Phoenix.Project1.Client.Utilities.RxExtensions;
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
    }
}