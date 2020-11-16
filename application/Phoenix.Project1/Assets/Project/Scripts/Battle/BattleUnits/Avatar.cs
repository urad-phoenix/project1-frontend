using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Phoenix.Pool;
using Phoenix.Project1.Client.Battles;
using UniRx;
using UnityEngine;
using UnityEngine.Timeline;

public class Avatar : MonoBehaviour
{
    [Serializable]
    public class DummyData
    {
        public DummyType DummyType;

        public Transform Dummy;
    }        
    
    [Serializable]
    public class VFXSource
    {
        public string Key;
        
        public GameObject Source;        
    }
    
    [Serializable]
    public class TimelineAssetSource
    {
        public ActionKey Action;
        
        public TimelineAsset TimelineAsset;
    }   

    [HideInInspector]
    public int InstanceID;

    [HideInInspector] 
    public int Location;

    public TimelineAssetSource[] TimelineAssets;

    public DummyData[] DummyDatas;

    public VFXSource[] VfxSources;

    private Dictionary<string, ObjectPool> _VFXPools;

    private CompositeDisposable _Disposable;

    [SerializeField]
    private CameraGroup _CameraGroup;

    public Avatar()
    {
        _Disposable = new CompositeDisposable();
        _VFXPools = new Dictionary<string, ObjectPool>();
    }

    private void Start()
    {
        Init();
    }

    public Transform GetDummy(string key)
    {
        try
        {
            var data = DummyDatas.First(x => x.DummyType.ToString() == key);
                
            return data.Dummy;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return this.transform;
    }

    public CinemachineVirtualCamera GetVirtualCamera(int index)
    {
        try
        {
            var cameraData = _CameraGroup.CameraDatas.First(x => x.Index == index);

            return cameraData.VirtualCamera;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }        
    }

    public void Init()
    {   
        foreach (var source in VfxSources)
        {
            var pool = new ObjectPool(source.Key, source.Source, this.transform, 5);

            pool.OnAfterSpawn += _AfterSpawn;
            
            pool.Initialize();
            
            pool.Spawn();
            
            _VFXPools.Add(source.Key, pool);
        }        
    }

    private void _AfterSpawn(GameObject go)
    {
        go.SetActive(false);
    }

    public GameObject GetVFX(string key)
    {
        ObjectPool pool;

        if (_VFXPools.TryGetValue(key, out pool))
        {
            var go = pool.Get(false);

            var obs = go.OnParticleStoppedAsObserver(key);

            obs.Subscribe(_Recycle).AddTo(_Disposable);

            return go;
        }

        return null;
    }

    private void _Recycle(ObserverParticleStopped go)
    {
        ObjectPool pool;
        if (_VFXPools.TryGetValue(go.Key, out pool))
        {
            pool.Recycle(go.gameObject, true);
        }
    }

    private void OnDestroy()
    {
        foreach (var pool in _VFXPools)
        {
            pool.Value.Clear();
        }
        
        _VFXPools.Clear();
        _Disposable.Clear();
    }
}
