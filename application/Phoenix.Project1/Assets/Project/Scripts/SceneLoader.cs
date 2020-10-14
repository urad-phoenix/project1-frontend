using Phoenix.Project1.Addressable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UniRx;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace Phoenix.Project1.Client
{
    public class SceneHookers
        : Regulus.Utility.Singleton<SceneHookers>
    {
        readonly HashSet<int> _Numbers;
        public SceneHookers()
        {
            _Numbers = new HashSet<int>();
        }
        int _Sn;
        public int Allocate()
        {
            var num = _Sn++;
            _Numbers.Add(num);
            return num;
        }
        public void Free(int token)
        {
            _Numbers.Remove(token);
        }

        public bool Empty => _Numbers.Count == 0;

        public IObservable<int> WaitEmpty()
        {
            return UniRx.Observable.FromCoroutineValue<int>(_Wait);
        }

        private IEnumerator _Wait()
        {
            yield return new WaitForEndOfFrame();
            while (!Empty)
            {
                yield return new WaitForEndOfFrame();
            }

            yield break;
        }
    }
    public class SceneLoader 
    {
        
        SceneInstance _Scene;
        internal void Open(string scene_name)
        {
            
            var unloadObs = _Unload();
            var loadObs = from unloadDone in unloadObs
                          from handle in UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(scene_name,UnityEngine.SceneManagement.LoadSceneMode.Additive).AsObserver()
                          select handle;
            
            var completeObs = from handle in loadObs
                              from _ in _Percent(handle.PercentComplete)
                              where handle.IsDone
                              select handle.Result;
            completeObs.Subscribe(_SetScene);
        }

        private IObservable<SceneInstance> _Unload()
        {
            if(_Scene.Scene.IsValid())
            {
                return from empty in SceneHookers.Instance.WaitEmpty().LastOrDefault()
                       from hnd in UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(_Scene).AsObserver()
                       where hnd.IsDone == true
                       select hnd.Result;
            }
            return UniRx.Observable.Return<SceneInstance>(_Scene);
        }

        internal void OpenDashboard()
        {
            Open("scene-dashboard");
        }

        private IObservable<Unit> _Percent(float percent)
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
