using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;

namespace Phoenix.Project1.Client.Utilities.RxExtensions
{    
    public static class RxDotweenExtensions
    {
        public static IObservable<Unit> OnCompleteAsObservable(this Tween self)
        {
            return Observable.FromEvent<TweenCallback>(
                h => h.Invoke,
                h => self.onComplete += h,
                h => self.onComplete -= h);
        }                
    }  
}