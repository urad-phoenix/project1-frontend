using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.Pool
{
    public class ObjectPool : IPool<GameObject>
    {
        private readonly Transform m_Parent;
        private readonly GameObject m_Source;
        private readonly object m_ID;
        private readonly int m_Size;

        private Stack<GameObject> m_Stack;
        private bool m_IsInitialized;
        private bool m_IsDispose;
        private bool m_IsBeforeRecycle;

        public Action<GameObject> OnAfterSpawn
        {
            get; set;
        }

        #region Exception

        private void ChekNotInitialized()
        {
            if(m_IsInitialized)
            {
                throw new ResourcesPoolException("ObjectPool is already initialized, Do not call this function when ObjectPool is initialized ");
            }
        }

        private void ChekIsInitialized()
        {
            if(!m_IsInitialized)
            {
                throw new ResourcesPoolException("ObjectPool is not initialized, Please call Initialize before this function");
            }
        }

        private void CheckObjectIsNull<T>(T obj)
        {
            if(obj == null)
            {
                throw new ResourcesPoolException("Pool Recycle: can not recycle null object");
            }
        }

        #endregion

        public ObjectPool(object id, GameObject source, Transform parent, int size, bool isBeforeRecycle = true)
        {
            m_ID = id;
            m_Source = source;
            m_Parent = parent;
            m_Size = size;
            m_Stack = new Stack<GameObject>();
            m_IsBeforeRecycle = isBeforeRecycle;
        }

        private GameObject Create()
        {
            if(m_Source == null)
                throw new ResourcesPoolException("ObjectPool source is null, Key :" + GetKey());

            var obj = GameObject.Instantiate(m_Source);

            if(m_Parent != null)
                obj.transform.SetParent(m_Parent);

            OnAfterSpawn?.Invoke(obj);

            if(m_IsBeforeRecycle)
                OnBeforeRecycle(obj);

            return obj;
        }

        private void OnClear(GameObject obj)
        {
            if(obj == null)
                return;

            UnityEngine.Object.Destroy(obj);
        }

        private void OnBeforeGet(GameObject obj)
        {
            if(!obj.gameObject.activeSelf)
                obj.gameObject.SetActive(true);
        }

        private void OnBeforeRecycle(GameObject obj)
        {
            if(obj.activeSelf)
                obj.SetActive(false);
        }

        public void Initialize()
        {
            ChekNotInitialized();
            m_IsDispose = false;
            m_IsInitialized = true;
        }

        public void Spawn()
        {
            ChekIsInitialized();

            for(int i = 0; i < m_Size; ++i)
            {
                var obj = Create();

                m_Stack.Push(obj);
            }
        }

        public GameObject Get(bool isDoBefore)
        {
            ChekIsInitialized();

            GameObject obj;
            if(m_Stack.Count == 0)
            {
                obj = Create();
            }
            else
            {
                obj = m_Stack.Pop();
            }

            if(isDoBefore)
                OnBeforeGet(obj);

            return obj;
        }

        public void Recycle(GameObject obj, bool isDoBefor)
        {
            ChekIsInitialized();

            CheckObjectIsNull(obj);

            var poolObj = obj;

            if(isDoBefor)
                OnBeforeRecycle(poolObj);

            if(!m_Stack.Contains(poolObj))
                m_Stack.Push(poolObj);
        }

        public void Clear()
        {
            ChekIsInitialized();

            if(m_Stack == null)
                return;

            while(m_Stack.Count != 0)
            {
                var obj = m_Stack.Pop();
                OnClear(obj);
            }
        }

        public int GetCount()
        {
            return m_Stack.Count;
        }

        public object GetKey()
        {
            return m_ID;
        }

        private void Dispose(bool disposing)
        {
            if(!m_IsDispose)
            {
                if(disposing)
                {
                    Clear();
                }
            }
            m_IsDispose = true;
            m_IsInitialized = false;
        }

        public void Dispose()
        {
            ChekIsInitialized();

            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
