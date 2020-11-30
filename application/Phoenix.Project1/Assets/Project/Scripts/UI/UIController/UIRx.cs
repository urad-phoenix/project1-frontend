
using System;
using UnityEngine;
using UniRx;

namespace Phoenix.Project1.Client.UI
{
    public static class UIRx
    {
        public static IObservable<Unit> FadeInAsObserver(this Component component)
        {
            var fade = component.GetComponent(typeof(UIFadeBase)) as UIFadeBase;

            return fade ? fade.FadeIn() : Observable.Return(Unit.Default);
        }
        public static IObservable<Unit> FadeOutAsObserver(this Component component)
        {
            var fade = component.GetComponent(typeof(UIFadeBase)) as UIFadeBase;

            return fade ? fade.FadeOut() : Observable.Return(Unit.Default);
        }
        public static void SetLayer(this Component component, UILayer layer)
        {
            UILayerController.Instance.SetToLayer(layer, component);
        }

        public static Transform GetLayer(UILayer layer)
        {
            if (UILayerController.Instance == null)
                return null;
            
            return UILayerController.Instance.GetCanvas(layer).transform;
        }

        public static void SetTransformParent(this Transform transform, Transform parent)
        {
            UILayerController.Instance.SetToParent(transform, parent);
        }        
    }
}
