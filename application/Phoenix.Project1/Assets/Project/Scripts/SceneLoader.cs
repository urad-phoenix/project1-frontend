using Phoenix.Project1.Addressable;
using System;
using System.Security.Cryptography;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace Phoenix.Project1.Client
{
    public class SceneLoader 
    {
        
        SceneInstance _Scene;
        internal void Open(string scene_name)
        {
            var unloadObs = _Unload();
            var loadObs = from unloadDone in unloadObs
                          from handle in UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(scene_name,UnityEngine.SceneManagement.LoadSceneMode.Additive).AsObserver()
                          select handle;

            /*var percentObs = from handle in loadObs
                             select handle.PercentComplete;
            percentObs.Subscribe(_PercentUI);*/

            var completeObs = from handle in loadObs
                              from _ in _PercentUI(handle.PercentComplete)
                              where handle.IsDone
                              select handle.Result;
            completeObs.Subscribe(_SetScene);
        }

        private IObservable<SceneInstance> _Unload()
        {
            if(_Scene.Scene.IsValid())
            {
                return from hnd in UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(_Scene).AsObserver()
                       where hnd.IsDone == true
                       select hnd.Result;
            }
            return UniRx.Observable.Return<SceneInstance>(_Scene);
        }

        internal void OpenDashboard()
        {
            Open("scene-dashboard");
        }

        private IObservable<Unit> _PercentUI(float percent)
        {
            UnityEngine.Debug.Log($"{percent}");
            return UniRx.Observable.Return(Unit.Default);
        }

        private void _SetScene(SceneInstance scene)
        {
            _Scene = scene;
        }

        internal void OpenLogin()
        {
            Open("scene-login");
        }

        internal void Close()
        {
            _Unload().Subscribe();
        }
    }

}
