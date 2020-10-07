using System;
using UniRx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Project.Scripts
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
