using System;
using UnityEngine;
using UnityEngine.Timeline;

namespace Phoenix.Playables.Markers
{  
    [Serializable]
    [TrackBindingType(typeof(PlayableReceiver))]
    [TrackColor(0.25f, 0.25f, 0.25f)]
    [ExcludeFromPreset]
    public class PlayableMarkerTrack : MarkerTrack {}
}