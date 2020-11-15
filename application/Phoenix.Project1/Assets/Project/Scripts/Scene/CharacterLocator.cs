using Phoenix.Project1.Client.Battles;
using UnityEngine;

namespace TP.Scene.Locators
{
    public class CharacterLocator : MonoBehaviour
    {
        public int Index;
    
        private int _InstanceID;
        
        private Role _Role;

        private void Start()
        {            
            _Role = GetComponentInChildren<Role>();
            _Role.Location = Index;
        }

        public int GetInstanceID()
        {
            return _InstanceID;
        }

        public void SetInstanceID(int id)
        {
            _InstanceID = id;
            _Role.ID = id;
        }

        public Role GetRole()
        {
            return _Role;
        }    
        
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