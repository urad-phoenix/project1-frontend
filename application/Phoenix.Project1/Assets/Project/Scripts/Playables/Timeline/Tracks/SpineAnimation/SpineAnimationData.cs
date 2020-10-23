using System;
using System.Collections.Generic;
using Phoenix.Playables.Markers;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables
{
    [Serializable]
    public class SpineAnimationData : BaseBehaviour
    {
        public string Name;
        public int Track;
        public bool IsLoop;
        [HideInInspector]
        public float Duration;
        [HideInInspector]
        public bool IsFirstFrameHappened;
        [HideInInspector]
        public string ReturnName;
        [HideInInspector]
        public bool IsReturnToSpecifyState;      
    }    
}
