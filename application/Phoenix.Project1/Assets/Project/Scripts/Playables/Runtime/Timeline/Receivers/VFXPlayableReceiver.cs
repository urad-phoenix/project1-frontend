using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables.Markers
{
    public class VFXPlayableReceiver : INotificationReceiver
    {
        public void OnNotify(Playable origin, INotification notification, object context)
        {
            var notifier = notification is VFXNotification ? (VFXNotification)notification : new VFXNotification() {Name = "non"} ;
                        
            Debug.Log($"notification type {notification.GetType().Name} id {notification.id}, name {notifier.Name} playable {origin.ToString()}");
        }
    }
}