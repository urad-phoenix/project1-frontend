using UnityEngine;

namespace TP.Scene.Locators
{
    public class CharacterLocator : MonoBehaviour
    {
        public int Index;
        
        #if UNITY_EDITOR
        [Header("Scene Editor")]
        [SerializeField]
        public Color Color = Color.blue;       
        
        [SerializeField]
        public float LabelShownPos = 0.5f;

        [SerializeField] 
        public int FontMaxSize = 10;
        
        [SerializeField] 
        public int FontMinSize = 3;

        [SerializeField] 
        public float ShowSize = 0.3f;         

        private void OnDrawGizmos()
        {            
            Gizmos.color = Color;
            Gizmos.DrawCube(transform.position, new Vector3(ShowSize, ShowSize, ShowSize));          
        }   
        #endif
    }
}