using System;
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
    }        
}
