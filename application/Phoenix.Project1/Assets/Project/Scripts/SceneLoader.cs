using Phoenix.Project1.Addressable;
using System;

using System.Collections.Generic;

using UniRx;

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

            
            

            var all = new List<IObservable<SceneInstance>>();
            all.AddRange(_Unload());
            all.AddRange(_Load(scene_names));
            UniRx.Observable.Merge(all).Subscribe(_AddScene);
        }

        private List<IObservable<SceneInstance>> _Load(string[] scene_names)
        {
            var length = scene_names.Length;
            var instances = new List<IObservable<SceneInstance>>();
            for (int i = 0; i < length; i++)
            {
                var sceneName = scene_names[i];
                var obs = from hnd in UniRx.Observable.Defer(() => UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive).AsObserver() )
                          select hnd.Result;
                instances.Add(obs)  ;
            }
            return instances;
        }

        private void _AddScene(SceneInstance obj)
        {
            if(obj.Scene.IsValid())
                _Scenes.Push(obj);
        }

        private List<IObservable<SceneInstance>> _Unload()
        {


            var length = _Scenes.Count;
            var instances = new List<IObservable<SceneInstance>>();
            while (_Scenes.Count > 0)
            {
                var instance = _Scenes.Pop();
                var obs = from hnd in UniRx.Observable.Defer(() => UnityEngine.AddressableAssets.Addressables.UnloadSceneAsync(instance).AsObserver())
                          select hnd.Result;
                instances.Add(obs) ;
            }
            return instances;


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

