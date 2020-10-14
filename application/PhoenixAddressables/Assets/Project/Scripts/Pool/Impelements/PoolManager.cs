using System;
using System.Collections.Generic;

namespace Phoenix.Pool
{
    public class PoolManager : IDisposable
    {
        private Dictionary<object, IPool> _PoolMap = new Dictionary<object, IPool>();

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

        private void _CheckHaveNoKey(object poolKey)
        {
            if(poolKey == null || !_PoolMap.ContainsKey(poolKey))
            {
                throw new ResourcesPoolException(
                    "PoolManager has no this key, Please make sure you call Add() before this function " +
                    poolKey.ToString());
            }
        }

        private void _CheckNotInitialized()
        {
            if(!_IsInitialized)
            {
                throw new ResourcesPoolException(
                    "PoolManager is not initialized. Please call Initialize() before call this function");
            }
        }

        public IPool AddPool(IPool pool)
        {
            _CheckNotInitialized();
            var poolKey = pool.GetKey();
            if(_PoolMap.ContainsKey(pool.GetKey()))
            {
                throw new ResourcesPoolException(
                    "Pool : " + poolKey + " is already added.");
            }
            else
            {
                _PoolMap.Add(poolKey, pool);
            }

            pool.Initialize();

            return pool;
        }

        public IPool GetPool(object poolKey)
        {
            _CheckNotInitialized();

            _CheckHaveNoKey(poolKey);

            return _PoolMap[poolKey];
        }

        public bool IsContainsPool(object poolKey)
        {
            return _PoolMap.ContainsKey(poolKey);
        }

        public T GetObject<T>(object poolKey, bool isDoBefore = true) where T : class
        {
            _CheckHaveNoKey(poolKey);

            var pool = _PoolMap[poolKey] as IPool<T>;

            if(pool == null)
            {
                throw new ResourcesPoolException(
                    "PoolManager GetObject : Object type " + typeof(T).Name + " Dose not match the pool object type ");
            }

            return pool.Get(isDoBefore);
        }

        public bool TryGetObject<T>(object poolKey, out T obj, bool isDoBefore = true) where T : class
        {
            if(_PoolMap.TryGetValue(poolKey, out var pool))
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
            _CheckNotInitialized();

            _CheckHaveNoKey(poolKey);

            var pool = _PoolMap[poolKey];
            pool.Dispose();
            _PoolMap.Remove(poolKey);
        }

        public void RemoveAllPools()
        {
            _CheckNotInitialized();

            foreach(var pool in _PoolMap)
            {
                pool.Value.Dispose();
            }

            _PoolMap.Clear();
        }

        public void Recycle<T>(object poolKey, T poolObject, bool isDoBefore = true)
        {
            _CheckHaveNoKey(poolKey);

            var pool = _PoolMap[poolKey] as IPool<T>;

            if(pool == null)
            {
                throw new ResourcesPoolException(
                    "PoolManager Recycle : Object type " + typeof(T).Name + " Dose not match the pool object type ");
            }

            pool.Recycle(poolObject, isDoBefore);
        }

        private void _Dispose(bool disposing)
        {
            if(!_IsDispose)
            {
                if(disposing)
                {
                    foreach(var pool in _PoolMap)
                    {
                        pool.Value.Dispose();
                    }

                    _PoolMap.Clear();
                }
            }
            _IsDispose = true;
            _IsInitialized = false;
        }

        public void Dispose()
        {
            _CheckNotInitialized();

            _Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
