using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.Pool
{
    public class ObjectPool : IPool<GameObject>
    {
        private readonly Transform _Parent;
        private readonly GameObject _Source;
        private readonly object _ID;
        private readonly int _Size;

        private Stack<GameObject> _Stack;
        private bool _IsInitialized;
        private bool _IsDispose;
        private bool _IsBeforeRecycle;

        public Action<GameObject> OnAfterSpawn
        {
            get; set;
        }

        #region Exception

        private void _ChekNotInitialized()
        {
            if(_IsInitialized)
            {
                throw new ResourcesPoolException("ObjectPool is already initialized, Do not call this function when ObjectPool is initialized ");
            }
        }

        private void _ChekIsInitialized()
        {
            if(!_IsInitialized)
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
            _ID = id;
            _Source = source;
            _Parent = parent;
            _Size = size;
            _Stack = new Stack<GameObject>();
            _IsBeforeRecycle = isBeforeRecycle;
        }

        private GameObject Create()
        {
            if(_Source == null)
                throw new ResourcesPoolException("ObjectPool source is null, Key :" + GetKey());

            var obj = GameObject.Instantiate(_Source);

            if(_Parent != null)
                obj.transform.SetParent(_Parent);

            OnAfterSpawn?.Invoke(obj);

            if(_IsBeforeRecycle)
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
            _ChekNotInitialized();
            _IsDispose = false;
            _IsInitialized = true;
        }

        public void Spawn()
        {
            _ChekIsInitialized();

            for(int i = 0; i < _Size; ++i)
            {
                var obj = Create();

                _Stack.Push(obj);
            }
        }

        public GameObject Get(bool isDoBefore)
        {
            _ChekIsInitialized();

            GameObject obj;
            if(_Stack.Count == 0)
            {
                obj = Create();
            }
            else
            {
                obj = _Stack.Pop();
            }

            if(isDoBefore)
                OnBeforeGet(obj);

            return obj;
        }

        public void Recycle(GameObject obj, bool isDoBefor)
        {
            _ChekIsInitialized();

            CheckObjectIsNull(obj);

            var poolObj = obj;

            if(isDoBefor)
                OnBeforeRecycle(poolObj);

            if(!_Stack.Contains(poolObj))
                _Stack.Push(poolObj);
        }

        public void Clear()
        {
            _ChekIsInitialized();

            if(_Stack == null)
                return;

            while(_Stack.Count != 0)
            {
                var obj = _Stack.Pop();
                OnClear(obj);
            }
        }

        public int GetCount()
        {
            return _Stack.Count;
        }

        public object GetKey()
        {
            return _ID;
        }

        private void _Dispose(bool disposing)
        {
            if(!_IsDispose)
            {
                if(disposing)
                {
                    Clear();
                }
            }
            _IsDispose = true;
            _IsInitialized = false;
        }

        public void Dispose()
        {
            _ChekIsInitialized();

            _Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
