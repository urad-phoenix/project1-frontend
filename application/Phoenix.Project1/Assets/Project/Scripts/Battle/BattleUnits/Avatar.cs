using System;
using Phoenix.Project1.Client.Battles;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.VFX;

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

    public TimelineAssetSource[] TimelineAssets;

    public DummyData[] DummyDatas;

    public VFXSource[] VfxSources;
}
