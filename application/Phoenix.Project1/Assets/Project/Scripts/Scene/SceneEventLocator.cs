using UnityEngine;

namespace TP.Scene.Locators
{
    public class SceneEventLocator : MonoBehaviour
    {
        public int Index;
        
        #if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {                   
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(this.transform.position, 0.2f);
        }
        
        #endif
    }
}