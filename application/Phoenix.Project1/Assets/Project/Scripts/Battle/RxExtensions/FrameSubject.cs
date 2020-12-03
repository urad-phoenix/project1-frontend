using System;
using Phoenix.Project1.Battles;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using UniRx;
using UniRx.Operators;
using Time = UnityEngine.Time;

namespace Phoenix.Project1.Client.Battles
{
    public class FrameSubject : OperatorObservableBase<int>
    {
        readonly IObservable<int> source;
        readonly int _Frame;

        public FrameSubject(IObservable<int> source, int frame) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this._Frame = frame;
        }

        protected override IDisposable SubscribeCore(IObserver<int> observer, IDisposable cancel)
        {
            return new Frame(this, observer, cancel).Run();
        }

        class Frame : OperatorObserverBase<int, int>
        {
            readonly FrameSubject parent;
            readonly object gate = new object();
            int latestValue = default(int);
            bool isUpdated = false;
            bool isCompleted = false;
            
            readonly TimePusher _Pusher;
            
            //readonly TimeCounter _Counter;

            private int _Frame;
            
            SingleAssignmentDisposable sourceSubscription;

            private IDisposable _UpdateDisposable;

            public Frame(FrameSubject parent, IObserver<int> observer, IDisposable cancel) : base(observer, cancel)
            {
                _Pusher = new TimePusher();
                //_Counter = new TimeCounter();
                this.parent = parent;

                _Frame = parent._Frame;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                _UpdateDisposable = UniRx.Observable.EveryUpdate().Subscribe(frame => OnNext((int)frame));

                return sourceSubscription;
            }
            
            int Advance()
            {
                var frames = _Pusher.Advance(Time.deltaTime);
                //_Counter.Reset();
                _Frame += frames;
            
                return _Frame;
            }

            public override void OnNext(int value)
            {
                lock (gate)
                {
                    latestValue = value;
                    isUpdated = true;
                    
                    if (isUpdated)
                    {
                        var frame = Advance();
                        isUpdated = false;
                        observer.OnNext(frame);
                    }

                    if (isCompleted)
                    {
                        try
                        {
                            observer.OnCompleted();
                        }
                        finally
                        {
                            Dispose();
                        }
                    }
                }
            }                       

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try
                    {
                        base.observer.OnError(error);                       
                    }
                    finally
                    {
                        _UpdateDisposable.Dispose();
                        sourceSubscription.Dispose();
                        Dispose();
                    }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    try
                    {
                        isCompleted = true;
                      
                        base.observer.OnCompleted();                                             
                    }
                    finally
                    {
                        _UpdateDisposable.Dispose();
                        sourceSubscription.Dispose();
                        Dispose();
                    }
                }
            }           
        }
    }

    public static class FrameSubjectRx
    {
        public static IObservable<int> OnFrameUpdateAsObserver(this IObservable<int> source, int frame)
        {
            return new FrameSubject(source, frame);
        }
    }
}