using System;
using Phoenix.Project1.Common.Battles;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class ActorTimelineObservable :  UniRx.Operators.OperatorObservableBase<ActorFrameMotion>
    {
        public class FrameData
        {
            public ActorFrameMotion Motion;
            public int StartFrame;
            public int CurrentFrame;
        }
        
        private FrameData _FrameData;
        
        public ActorTimelineObservable(ActorFrameMotion value, int startFrame, int currentFrame) : base(false)
        {
            
            _FrameData = new FrameData();            
            _FrameData.Motion = value;
            _FrameData.StartFrame = startFrame;
            _FrameData.CurrentFrame = currentFrame;
        }

        protected override IDisposable SubscribeCore(IObserver<ActorFrameMotion> observer, IDisposable cancel)
        {
            return new ActorObserver(_FrameData, observer, cancel).Run();
        }
        
        class ActorObserver : UniRx.Operators.OperatorObserverBase<ActorFrameMotion, ActorFrameMotion>
        {
            readonly CompositeDisposable _CancellationToken = new CompositeDisposable();
            bool isCompleted;
            
            private Subject<int> _Frame;        
            
            private FrameData _FrameData;
            
            public ActorObserver(FrameData frameData, IObserver<ActorFrameMotion> observer, IDisposable cancel) : base(observer, cancel)
            {                
                _FrameData = frameData;
               
                isCompleted = false;             
            }

            private void Update(int frame)
            {              
                if(isCompleted)
                    return;

                if (frame >= _FrameData.StartFrame)
                {                  
                    //Debug.Log($"ActorObserver {_FrameData.Motion.MotionId} currentFrame {frame}, target frame {_FrameData.StartFrame}");
                    observer.OnNext(_FrameData.Motion);                        
                    OnCompleted();
                }
            }
           
            public IDisposable Run()
            {               
                _Frame = new Subject<int>();               
                
                var obs = FrameSubjectRx.OnFrameUpdateAsObserver(_Frame.AsObservable(), _FrameData.CurrentFrame);

                obs.ObserveOnMainThread().Subscribe(frame => Update(frame)).AddTo(_CancellationToken);

                return this;
            }         

            public override void OnNext(ActorFrameMotion value)
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
                    _CancellationToken.Clear();
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
                    _CancellationToken.Clear();
                }
            }
        }
    }
    
    public static class ActorTimelineRx
    {
        public static IObservable<ActorFrameMotion> OnActorObserver(ActorFrameMotion motion, int startFrame, int currentFrame)
        {                                  
            return new ActorTimelineObservable(motion, startFrame, currentFrame); 
        }
    }
}