using System;

namespace Regulus.Remote.Reactive
{
    internal class OnceRemoteReturnValueEvent<T> : UniRx.Operators.OperatorObservableBase<T>, IDisposable
    {
        private Value<T> _Return;
        private IDisposable _Cancel;
        IObserver<T> _Observer;
        public OnceRemoteReturnValueEvent(Value<T> ret) : base(false)
        {
            this._Return = ret;
        }

        private void _OnValue(T obj)
        {
            _Observer.OnNext(obj);
            _Observer.OnCompleted();
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            _Cancel = cancel;
            _Observer = observer;
            _Return.OnValue += _OnValue;
            return this;
        }

        void IDisposable.Dispose()
        {
            _Return.OnValue -= _OnValue;
            _Cancel.Dispose();
        }
    }
}