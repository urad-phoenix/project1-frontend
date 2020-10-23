using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Phoenix.Playables.Attribute;
using Phoenix.Playables.Markers;
using Phoenix.Playables.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Playables
{
    public abstract class BaseTrack : TrackAsset
    {        
        public void SetMarker(TimelineClip clip, BaseBehaviour behaviour)
        {                        
            var markers = GetMarkers();
            
            List<BasePlayableMarker> clipMarkers = new List<BasePlayableMarker>();
            
            foreach (var marker in markers)
            {
                if (marker.time <= clip.end && marker.time >= clip.start)
                {
                    //Debug.Log($"Time {marker.time}");

                    if (marker is BasePlayableMarker)
                    {
                        clipMarkers.Add(marker as BasePlayableMarker);
                    }
                }       
            }

            behaviour.SetMarkers(clipMarkers);
        }
    }
    
    [Serializable]
    public abstract class BaseBehaviour : PlayableBehaviour
    {
        public List<BasePlayableMarker> Markers;
        
        private List<BasePlayableMarker> _UsedMarkers = new List<BasePlayableMarker>();

        private INotificationReceiver _Receiver;               

        public INotificationReceiver Receiver => _Receiver;      

        public void SendNotification(BasePlayableNotification notification, Playable playable, PlayableOutput output, double time)
        {            
            if(_Receiver == null)
                return;
            
            var marker = Markers.Find(x => x.time < time);

            if (_UsedMarkers.Exists(x => x == marker))
            {
                return;
            }

            if (marker)
            {
                //Debug.Log("Time" + time + " marker time " + marker.time);
                
                _UsedMarkers.Add(marker);

                notification = _Binding(notification, marker);
                
                output.PushNotification(playable, notification);
            }
        }

        public void AddNotification(Playable playable, INotificationReceiver receiver)
        {
            if(!HasMarkers())
                return;
            
            if(receiver == null)
                return;
            
            _Receiver = receiver;                       

            var count = playable.GetGraph().GetOutputCount();

            for (int i = 0; i < count; ++i)
            {
                var output = playable.GetGraph().GetOutput(i);
                //Debug.Log("AddNotificationReceiver");
                output.AddNotificationReceiver(_Receiver);                
            }
        }

        public bool HasMarkers()
        {
            return Markers.Count != 0;
        }

        public void RemoveNotification(Playable playable)
        {          
            //Debug.Log("RemoveNotification");
            if(_Receiver == null)
                return;            
            
            var count = playable.GetGraph().GetOutputCount();

            _UsedMarkers.Clear();
            
            for (int i = 0; i < count; ++i)
            {
                var output = playable.GetGraph().GetOutput(i);
                
                output.RemoveNotificationReceiver(_Receiver);
            }
        }

        public void SetMarkers(List<BasePlayableMarker> markers)
        {
            if (Markers == null)
            {
                Markers = new List<BasePlayableMarker>();
            }
            
            Markers.Clear();
                        
            Markers.AddRange(markers);
        }

        private N _Binding<N, T>(N notification, T marker) where N : BasePlayableNotification 
            where T : BasePlayableMarker
        {
            var markerType = marker.GetType();
            var markerFields = markerType.GetPublicFields();

            List<FieldInfo> bindingFields = new List<FieldInfo>();
            
            for (int i = 0; i < markerFields.Length; ++i)
            {
                var field = markerFields[i];
                
                var attribute = field.GetCustomAttribute<NotificationBindingAttribute>();

                if (attribute != null)
                {
                    bindingFields.Add(field);
                }
            }
           
            var notifyFields = notification.GetType().GetPublicFields();

            foreach (var field in notifyFields)
            {
                try
                {
                    var bindField = bindingFields.First(x => x.Name == field.Name);

                    field.SetValue(notification, bindField.GetValue(marker));
                }
                catch
                {                    
                }                 
            }

            return notification;
        }
    }
}