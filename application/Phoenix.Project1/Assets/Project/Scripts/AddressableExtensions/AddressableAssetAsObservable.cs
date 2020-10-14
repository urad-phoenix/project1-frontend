using System;
using System.Collections;
using UniRx;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Phoenix.Project1.Addressable
{
    public static class AddressableAssetRx
    {
        public static IObservable<AsyncOperationHandle<T>> AsObserver<T>(this AsyncOperationHandle<T> addressable)
        {
            return Observable.FromEvent<AsyncOperationHandle<T>>(
                handler => addressable.Completed += handler,
                handler => addressable.Completed -= handler);
        }

        public static IObservable<AsyncOperationHandle<T>> AsHandleObserver<T>(this AsyncOperationHandle<T> async_operation)
        {
            return Observable.FromCoroutine<AsyncOperationHandle<T>>((observer, cancellationToken) => RunAsyncOperation(async_operation, observer, cancellationToken));
        }

        static IEnumerator RunAsyncOperation<T>(AsyncOperationHandle<T> async_operation, IObserver<AsyncOperationHandle<T>> observer, System.Threading.CancellationToken cancellationToken)
        {
            while (!async_operation.IsDone && !cancellationToken.IsCancellationRequested)
            {
                observer.OnNext(async_operation);
                yield return null;
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                observer.OnNext(async_operation); 
                observer.OnCompleted();
            }
        }

    }        
}

