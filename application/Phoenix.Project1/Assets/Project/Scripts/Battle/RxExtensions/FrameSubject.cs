using System;
using Phoenix.Project1.Battles;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote;
using Regulus.Utility;
using UniRx;
using UniRx.Operators;

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
            
            readonly TimeCounter _Counter;

            private int _Frame;
            
            SingleAssignmentDisposable sourceSubscription;

            public Frame(FrameSubject parent, IObserver<int> observer, IDisposable cancel) : base(observer, cancel)
            {
                _Pusher = new TimePusher();
                _Counter = new TimeCounter();
                this.parent = parent;

                _Frame = parent._Frame;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                var scheduling = UniRx.Observable.EveryUpdate()
                    .ObserveOnMainThread().Subscribe(frame => OnNext((int)frame));

                return StableCompositeDisposable.Create(sourceSubscription, scheduling);
            }
            
            int Advance()
            {
                var frames = _Pusher.Advance(_Counter.Second);
                _Counter.Reset();
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
                        sourceSubscription.Dispose();
                    }
                    finally
                    {
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
                        sourceSubscription.Dispose();
                    }
                    finally
                    {
                        Dispose();
                    }
                }
            }

            class FrameTick : IObserver<long>
            {
                readonly Frame parent;

                public FrameTick(Frame parent)
                {
                    this.parent = parent;
                }

                public void OnCompleted()
                {
                }

                public void OnError(Exception error)
                {
                }

                public void OnNext(long _)
                {
                    lock (parent.gate)
                    {
                        if (parent.isUpdated)
                        {
                            var value = parent.latestValue;
                            parent.isUpdated = false;
                            parent.observer.OnNext(value);
                        }

                        if (parent.isCompleted)
                        {
                            try
                            {
                                parent.observer.OnCompleted();
                            }
                            finally
                            {
                                parent.Dispose();
                            }
                        }
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