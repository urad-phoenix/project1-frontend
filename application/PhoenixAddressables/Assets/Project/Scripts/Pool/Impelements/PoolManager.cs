using System;
using System.Collections.Generic;

namespace Phoenix.Pool
{
    public class PoolManager : IDisposable
    {
        private Dictionary<object, IPool> m_PoolMap = new Dictionary<object, IPool>();

        private bool _IsInitialized;

        private bool _IsDispose;

        public void Initialize()
        {
            if(_IsInitialized)
            {
                throw new ResourcesPoolException("PoolManager is already initialized. Do not call this function when PoolManager isInitialized");
            }

            _IsInitialized = true;
            _IsDispose = false;
        }

        public bool IsInitialized
        {
            get
            {
                return _IsInitialized;
            }
        }

        private void CheckHaveNoKey(object poolKey)
        {
            if(poolKey == null || !m_PoolMap.ContainsKey(poolKey))
            {
                throw new ResourcesPoolException(
                    "PoolManager has no this key, Please make sure you call Add() before this function " +
                    poolKey.ToString());
            }
        }

        private void CheckNotInitialized()
        {
            if(!_IsInitialized)
            {
                throw new ResourcesPoolException(
                    "PoolManager is not initialized. Please call Initialize() before call this function");
            }
        }

        public IPool AddPool(IPool pool)
        {
            CheckNotInitialized();
            var poolKey = pool.GetKey();
            if(m_PoolMap.ContainsKey(pool.GetKey()))
            {
                throw new ResourcesPoolException(
                    "Pool : " + poolKey + " is already added.");
            }
            else
            {
                m_PoolMap.Add(poolKey, pool);
            }

            pool.Initialize();

            return pool;
        }

        public IPool GetPool(object poolKey)
        {
            CheckNotInitialized();

            CheckHaveNoKey(poolKey);

            return m_PoolMap[poolKey];
        }

        public bool IsContainsPool(object poolKey)
        {
            return m_PoolMap.ContainsKey(poolKey);
        }

        public T GetObject<T>(object poolKey, bool isDoBefore = true) where T : class
        {
            CheckHaveNoKey(poolKey);

            var pool = m_PoolMap[poolKey] as IPool<T>;

            if(pool == null)
            {
                throw new ResourcesPoolException(
                    "PoolManager GetObject : Object type " + typeof(T).Name + " Dose not match the pool object type ");
            }

            return pool.Get(isDoBefore);
        }

        public bool TryGetObject<T>(object poolKey, out T obj, bool isDoBefore = true) where T : class
        {
            if(m_PoolMap.TryGetValue(poolKey, out var pool))
            {
                if(pool is IPool<T>)
                {
                    var getPool = pool as IPool<T>;
                    obj = getPool.Get(isDoBefore);
                    return true;
                }
            }

            obj = null;
            return false;
        }

        public void RemovePool(object poolKey)
        {
            CheckNotInitialized();

            CheckHaveNoKey(poolKey);

            var pool = m_PoolMap[poolKey];
            pool.Dispose();
            m_PoolMap.Remove(poolKey);
        }

        public void RemoveAllPools()
        {
            CheckNotInitialized();

            foreach(var pool in m_PoolMap)
            {
                pool.Value.Dispose();
            }

            m_PoolMap.Clear();
        }

        public void Recycle<T>(object poolKey, T poolObject, bool isDoBefore = true)
        {
            CheckHaveNoKey(poolKey);

            var pool = m_PoolMap[poolKey] as IPool<T>;

            if(pool == null)
            {
                throw new ResourcesPoolException(
                    "PoolManager Recycle : Object type " + typeof(T).Name + " Dose not match the pool object type ");
            }

            pool.Recycle(poolObject, isDoBefore);
        }

        protected void Dispose(bool disposing)
        {
            if(!_IsDispose)
            {
                if(disposing)
                {
                    foreach(var pool in m_PoolMap)
                    {
                        pool.Value.Dispose();
                    }

                    m_PoolMap.Clear();
                }
            }
            _IsDispose = true;
            _IsInitialized = false;
        }

        public void Dispose()
        {
            CheckNotInitialized();

            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
