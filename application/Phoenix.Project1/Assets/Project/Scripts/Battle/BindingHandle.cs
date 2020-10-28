using System;
using Object = UnityEngine.Object;

namespace Phoenix.Project1.Client.Battles
{
    public struct BindingHandle
    {
        static readonly BindingHandle m_Null = new BindingHandle();

        public static BindingHandle Null
        {
            get { return m_Null; }
        }

        private Object _ReferenceObject;

        public bool IsValid()
        {
            return _ReferenceObject != null;
        }

        public Object GetReferenceObject()
        {
            return _ReferenceObject;
        }

        public void SetReferenceObject(Object obj)
        {
            _ReferenceObject = obj;
        }
        
        public bool IsPlayableOfType<T>()
        {
            return GetPlayableType() == typeof(T);
        }

        private Type GetPlayableType()
        {
            return _ReferenceObject.GetType();
        }
    }
}