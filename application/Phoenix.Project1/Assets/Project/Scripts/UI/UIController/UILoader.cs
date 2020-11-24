using System;
using System.Collections.Generic;
using Phoenix.Project1.Addressable;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public interface IUIQueryable
{
    GameObject Query(string name);
}

namespace Phoenix.Project1.Client.UI
{   
    
    public class UILoader : MonoBehaviour, IUIQueryable
    {       
        [Serializable]
        public class UIInfo
        {
            public string Name;
            public UILayer Layer;
        }

        internal class AssetData
        {
            public string Key;
            public AsyncOperationHandle<GameObject> Handle;
        }

        [SerializeField]
        private UIInfo[] _UIs;     
        
        readonly System.Collections.Generic.List<GameObject> _UIContainer;

        private System.Collections.Generic.List<IObservable<AsyncOperationHandle<GameObject>>> _LoadHandles;

        private CompositeDisposable _Disposable;

        public event Action LoadCompletedEvent;

        public readonly IUIQueryable Queryable;
            
        public UILoader()
        {
            Queryable = this;
            _UIContainer = new System.Collections.Generic.List<GameObject>();
            _LoadHandles = new System.Collections.Generic.List<IObservable<AsyncOperationHandle<GameObject>>>();
            _Disposable = new CompositeDisposable();
        }        

        public void Load()
        {
            var obs = Open(_UIs);
            
            obs.Subscribe(ob =>
                {
                    Debug.Log($"UI loaded ui count{ob.Length}");
                    
                    LoadCompletedEvent?.Invoke();
                });
        }

        internal IObservable<AssetData[]> Open(params UIInfo[] uis)
        {            
            _Unload();
            var handles = _Load(uis);

            var obs = from handle in UniRx.Observable.Merge(handles)
                from observable in _AddUI(handle, uis.Length)
                where observable
                select handle;

            return Observable.WhenAll(obs);          
        }

        private System.Collections.Generic.List<IObservable<AssetData>> _Load(UIInfo[] uis)
        {            
            var length = uis.Length;
            
            var instances = new System.Collections.Generic.List<IObservable<AssetData>>();

            var loadedList = new List<string>();
            
            for (int i = 0; i < length; i++)
            {
                var uiInfo = uis[i];
                
                if(loadedList.Exists(x => x == uiInfo.Name))
                    continue;
                
                var parent = UIRx.GetLayer(uiInfo.Layer);

                var obs = from hnd in UniRx.Observable.Defer(() =>
                        Addressables.InstantiateAsync(uiInfo.Name, parent).AsHandleObserver())
                    select new AssetData() {Key = uiInfo.Name, Handle = hnd};
                                             
                instances.Add(obs);
            }
            
            return instances;
        }

        private void _Unload()
        {
            while (_UIContainer.Count > 0)
            {
                var instance = _UIContainer[0];
                _UIContainer.RemoveAt(0);
                UnityEngine.AddressableAssets.Addressables.ReleaseInstance(instance);                
            }
            
            //UnityEngine.AddressableAssets.Addressables.Release(_LoadHandles);
            
            _LoadHandles.Clear();
        }

        private IObservable<bool> _AddUI(AssetData data, float count)
        {
            var percent = (data.Handle.PercentComplete + _UIs.Length) / count;
            
           // UnityEngine.Debug.Log($"ui {data.Key} ui load : {percent}% ");

            if (!data.Handle.IsDone)
            {
                return Observable.Return(false);
            }           
            
            data.Handle.Result.SetActive(false);
            
            data.Handle.Result.name = data.Key;          

            _UIContainer.Add(data.Handle.Result);

            return Observable.Return(true);
        }

        private void OnDisable()
        {
            _Unload();
            _Disposable.Clear();
        }

        GameObject IUIQueryable.Query(string name)
        {
            var go = _UIContainer.Find(x => x.name.Contains(name));
            
            return go;
        }            
    }
}