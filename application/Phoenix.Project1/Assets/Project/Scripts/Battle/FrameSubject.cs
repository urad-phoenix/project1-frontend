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
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public FrameSubject(IObservable<int> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
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
            SingleAssignmentDisposable sourceSubscription;

            public Frame(FrameSubject parent, IObserver<int> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                var scheduling = UniRx.Observable.EveryFixedUpdate()
                    .Subscribe(frame => OnNext((int)frame));

                return StableCompositeDisposable.Create(sourceSubscription, scheduling);
            }

            public override void OnNext(int value)
            {
                lock (gate)
                {
                    latestValue = value;
                    isUpdated = true;
                    
                    if (isUpdated)
                    {                        
                        isUpdated = false;
                        observer.OnNext(value);
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
                        Dispose();
                    }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isCompleted = true;
                    sourceSubscription.Dispose();
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
            return new FrameSubject(source, frame, FrameCountType.FixedUpdate);
        }
    }
}