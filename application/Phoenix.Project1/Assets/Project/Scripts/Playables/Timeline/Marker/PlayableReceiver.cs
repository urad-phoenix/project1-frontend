using UnityEngine;
using UnityEngine.Playables;

namespace Phoenix.Playables.Markers
{
    public class PlayableReceiver : INotificationReceiver
    {        
        public void OnNotify(Playable origin, INotification notification, object context)
        {                            
            var notifier = notification is SpineAnimationNotification ? (SpineAnimationNotification)notification : new SpineAnimationNotification() {Name = "non"} ;
                        
            Debug.Log($"notification type {notification} id {notification.id}, name {notifier.Name} playable {origin.ToString()}");
        }
    }
}