using System;
using UniRx;



namespace Regulus.Remote.Reactive
{
    public static class PropertyRx
    {

        internal class PropertyObservable<T> : UniRx.Operators.OperatorObservableBase<T>
        {
            private readonly IScheduler _Scheduler;
            private readonly Property<T> _Property;

            public PropertyObservable(Regulus.Remote.Property<T> property) : base(false)
            {
                
                _Scheduler = Scheduler.ThreadPool;
                this._Property = property;
            }
            protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
            {
                return _Scheduler.Schedule(new PropertyObserver(_Property,observer, cancel).Run);
            }

            class PropertyObserver : UniRx.Operators.OperatorObserverBase<T, T>
            {
                volatile bool _Enable;
                readonly Regulus.Remote.Property<T> _Property;
                T _Value;
                public PropertyObserver(Regulus.Remote.Property<T> property ,  IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
                {
                    _Property = property;
                    _Enable = true;
                    _Value = _Property.Value;
                }
                public override void OnCompleted()
                {
                    _Enable = false;
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }

                public override void OnError(Exception error)
                {
                    try { observer.OnError(error); }
                    finally { Dispose(); }
                }

                public override void OnNext(T value)
                {
                    try
                    {
                        base.observer.OnNext(value);
                    }
                    catch
                    {
                        Dispose();
                        throw;
                    }
                }

                internal void Run()
                {
                    
                    observer.OnNext(_Value);

                    var apr = new Regulus.Utility.AutoPowerRegulator(new Utility.PowerRegulator());
                    while(_Enable)
                    {
                        apr.Operate();
                        var v2 = _Property.Value;
                        if (!_Value.Equals(v2))
                        {
                            observer.OnNext(v2);
                            _Value = v2;
                        }                            
                    }
                    observer.OnCompleted();
                }
            }
        }
        public static IObservable<T> ChangeObservable<T>(this Regulus.Remote.Property<T> instance )
        {
            return new PropertyObservable<T>(instance);
        }

        
    }
}
