using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public struct BehaviourHandle
    {       
        private Object _ReferenceObject;
        
        public Object GetReferenceObject()
        {
            return _ReferenceObject;
        }

        public void SetReferenceObject(Object obj)
        {
            _ReferenceObject = obj;
        }
    }
}