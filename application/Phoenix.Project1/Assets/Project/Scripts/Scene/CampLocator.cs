using Phoenix.Project1.Client.Battles;
using UnityEngine;

namespace TP.Scene.Locators
{
    public class CampLocator : MonoBehaviour
    {
        public CampType CampType;       
        
        #if UNITY_EDITOR
        
        [Header("Scene Editor")]
        [SerializeField]
        public Color Color = Color.blue;

        [SerializeField]
        public float ShownPos = 1.8f;
        
        [SerializeField]
        public float LabelShownPos = 2.3f;

        [SerializeField] 
        public int FontMaxSize = 10;
        
        [SerializeField] 
        public int FontMinSize = 3;

        [SerializeField] 
        public float ShowSize = 0.3f;                            
        
        #endif
    }
   
}