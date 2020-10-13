using System;

namespace Phoenix.Pool
{
    public interface IPool : IDisposable
    {
        void Initialize();
        void Spawn();
        void Clear();
        int GetCount();
        object GetKey();
    }

    public interface IPool<TObject> : IPool
    {
        TObject Get(bool isDoBefore);
        void Recycle(TObject obj, bool isDoBefore);
        Action<TObject> OnAfterSpawn
        {
            get; set;
        }
    }
}