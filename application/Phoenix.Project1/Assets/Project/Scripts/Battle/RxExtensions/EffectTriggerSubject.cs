using System;
using System.Threading;
using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class EffectTriggerObservable : UniRx.Operators.OperatorObservableBase<Effect>
    {
        public class FrameEffect
        {
            public Effect Effect;
            public int Frame;
            public int CurrentFrame;
        }                

        private FrameEffect _FrameEffect;
        
        public EffectTriggerObservable(Effect value, int frame, int currentFrame)
            : base(false)
        {

            _FrameEffect = new FrameEffect();            
            _FrameEffect.Effect = value;
            _FrameEffect.Frame = frame;
            _FrameEffect.CurrentFrame = currentFrame;
        }                       

        protected override IDisposable SubscribeCore(IObserver<Effect> observer, IDisposable cancel)
        {
            return new EffectTriggerObserver(_FrameEffect, observer, cancel).Run();
        }

        class EffectTriggerObserver : UniRx.Operators.OperatorObserverBase<Effect, Effect>
        {         
            readonly CompositeDisposable _CancellationToken = new CompositeDisposable();
            bool isCompleted;
            
            private Subject<int> _Frame;        
            
            private FrameEffect _FrameEffect;

            private int _CurrentFrame;
            
            public EffectTriggerObserver(FrameEffect effect, IObserver<Effect> observer, IDisposable cancel) : base(observer, cancel)
            {                
                _FrameEffect = effect;

                _CurrentFrame = _FrameEffect.CurrentFrame;
                
                isCompleted = false;             
            }

            private void Update(int frame)
            {              
                if(isCompleted)
                    return;

                if (frame >= _FrameEffect.Frame)
                {                  
                    observer.OnNext(_FrameEffect.Effect);                        
                    OnCompleted();
                }
            }
           
            public IDisposable Run()
            {               
                _Frame = new Subject<int>();               
                
                var obs = FrameSubjectRx.OnFrameUpdateAsObserver(_Frame.AsObservable(), _CurrentFrame);

                var scheduling = obs.Subscribe(frame => Update(frame)).AddTo(_CancellationToken);
                
//                var scheduling = UniRx.Observable.EveryUpdate()
//                    .ObserveOnMainThread().Subscribe(frame => Update(_CurrentFrame++)).AddTo(_CancellationToken);
                
                return _CancellationToken;
               
            }

            public override void OnNext(Effect value)
            {               
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try
                {
                    observer.OnError(error);
                }
                finally
                {
                    Dispose(); 
                    _CancellationToken.Dispose();
                }
            }

            public override void OnCompleted()
            {
                try
                {
                    isCompleted = true;

                    observer.OnCompleted();
                }
                finally
                {
                    Dispose();
                    _CancellationToken.Dispose();
                }
            }
        }
    }

    public static class EffectTriggerRx
    {
        public static IObservable<Effect> OnEffectTriggerObserver(Effect effect, int frame, int currentFrame)
        {                                  
            return new EffectTriggerObservable(effect, frame, currentFrame); 
        }
    }
}