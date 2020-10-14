using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Profiling;
using Phoenix.Pool;

public class LoadTest : MonoBehaviour
{
    [SerializeField]
    private Transform _Parent;

    private List<GameObject> _Instances = new List<GameObject>();

    private List<AsyncOperationHandle> _AsyncOperationHandles = new List<AsyncOperationHandle>();

    private AsyncOperationHandle _InstnaceHandle;
    public void LoadByKey(string key)
    {                
        var handle = Addressables.LoadAssetsAsync<GameObject>(key, LoadCallback);

        _AsyncOperationHandles.Add(handle);

        handle.Completed += (async) =>
        {
            Debug.Log("key: " + key + " completed " + handle.Result.Count);            
        };
        
    }
    public void ReleaseLoad()
    {
        for(int i = 0; i < _AsyncOperationHandles.Count; ++i)
        {
            Addressables.Release(_AsyncOperationHandles[i]);
        }
        _AsyncOperationHandles.Clear();
    }

    private void LoadCallback<T>(T obj)
    {
        Debug.Log("LoadCallback completed " + obj.ToString());
    }
   
    public void LoadAndInstance(string key)
    {
        Profiler.BeginSample("LoadAndInstance");
        for(int i = 0; i < 1000; ++i)
        {
            var handle = Addressables.InstantiateAsync(key, _Parent);
        
            handle.Completed += (async) =>
            {       
                _Instances.Add(handle.Result);        
            };
        }
        Profiler.EndSample();
        //Profiler.BeginSample("LoadAndInstance");
        //var handle = Addressables.LoadAssetAsync<GameObject>(key);
        //
        //handle.Completed += h => {
        //
        //    for(int i = 0; i < 1000; ++i)
        //    {
        //        _Instances.Add(Instantiate(handle.Result, _Parent));
        //    }
        //};
        //
        //Profiler.EndSample();
    }
    public void DestroyInstance()
    {
        Profiler.BeginSample("DestroyInstance");
        for(int i = 0; i < _Instances.Count; ++i)
        {
            Addressables.ReleaseInstance(_Instances[i]);
            //Destroy(_Instances[i]);
        }

        _Instances.Clear();

        Profiler.EndSample();
    }    
}
