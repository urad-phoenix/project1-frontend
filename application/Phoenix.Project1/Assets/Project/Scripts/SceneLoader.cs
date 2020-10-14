using Phoenix.Project1.Addressable;
using System;

using System.Collections.Generic;
using System.Security.Cryptography;
using UniRx;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


namespace Phoenix.Project1.Client
{
    
    public class SceneLoader 
    {

        readonly System.Collections.Generic.Stack<SceneInstance> _Scenes;
            
        public SceneLoader()
        {
            _Scenes = new Stack<SceneInstance>();
        }
        internal void Open(params string[] scene_names)
        {
            var all = new List<IObservable<AsyncOperationHandle<SceneInstance>>>();
            all.AddRange(_Load(scene_names));

            var obs = from unloadDone in UniRx.Observable.Merge(_Unload()).LastOrDefault()
                        from handle in UniRx.Observable.Merge(all)
                        select handle ;

            obs.Subscribe( handle=> _AddScene(handle, scene_names.Length));
        }

        private List<IObservable<AsyncOperationHandle<SceneInstance>>> _Load(string[] scene_names)
        {
            
            var length = scene_names.Length;
            var instances = new List<IObservable<AsyncOperationHandle<SceneInstance>>>();
            for (int i = 0; i < length; i++)
            {
                var sceneName = scene_names[i];
                var obs = from hnd in UniRx.Observable.Defer(() => UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive).AsHandleObserver() )
                          select hnd;
                instances.Add(obs)  ;
            }
            return instances;
        }

        private List<IObservable<AsyncOperationHandle<SceneInstance>>> _Unload()
        {
            var length = _Scenes.Count;
            var instances = new List<IObservable<AsyncOperationHandle<SceneInstance>>>();
            while (_Scenes.Count > 0)
            {
                var instance = _Scenes.Pop();
                var obs = from hnd in UniRx.Observable.Defer(() => UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(instance).AsHandleObserver())
                          select hnd;
                instances.Add(obs);
            }
            return instances;
        }


        private void _AddScene(AsyncOperationHandle<SceneInstance> handle,float count)
        {
            var percent = (handle.PercentComplete + _Scenes.Count) / count;
            UnityEngine.Debug.Log($"scene load : {percent}% ");

            if (!handle.IsDone)
            {
                return;
            }

            if (!handle.Result.Scene.IsValid())
                return;
            _Scenes.Push(handle.Result);
        }


        internal void OpenDashboard()
        {
            Open("scene-dashboard","scene-test2");
        }

        

        

        internal void OpenLogin()
        {
            Open("scene-login", "scene-test1");
        }

        internal void Close()
        {
            UniRx.Observable.Merge(_Unload()).Subscribe();
        }
    }

}

