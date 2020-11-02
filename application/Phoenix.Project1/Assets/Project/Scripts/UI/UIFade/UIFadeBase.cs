using System;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public abstract class UIFadeBase : MonoBehaviour
    {
        [SerializeField]
        protected bool _IsFinishedDisable;
        public abstract IObservable<Unit> FadeIn();

        public abstract IObservable<Unit> FadeOut();                       
    }
}
