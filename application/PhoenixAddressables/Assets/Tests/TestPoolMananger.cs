using NUnit.Framework;
using Phoenix.Pool;
using UnityEngine;

namespace Tests
{
    [Category("PoolManagerTests")]
    public class TestPoolManager
    {
        private PoolManager m_MockPoolManager;

        [SetUp]
        public void SetupTestPoolManager()
        {
            m_MockPoolManager = new PoolManager();

            m_MockPoolManager.Initialize();

            var pool = new ObjectPool("TestPool_01", new GameObject(), null, 3);
            m_MockPoolManager.AddPool(pool);
        }

        [Test]
        public void TestPoolSpawn()
        {
            var pool = m_MockPoolManager.GetPool("TestPool_01");
            Assert.IsTrue(pool.GetCount() == 0);
            pool.Spawn();
            Assert.IsTrue(pool.GetCount() == 3);
        }

        [Test]
        public void GetObject()
        {
            var pool = m_MockPoolManager.GetPool("TestPool_01");
            Assert.IsTrue(pool.GetCount() == 0);
            pool.Spawn();

            Assert.IsTrue(pool.GetCount() == 3);

            var obj = m_MockPoolManager.GetObject<GameObject>("TestPool_01");

            Assert.IsNotNull(obj);

            Assert.IsTrue(pool.GetCount() == 2);
        }

        [Test]
        public void RecycleObject()
        {
            var pool = m_MockPoolManager.GetPool("TestPool_01");
            Assert.IsTrue(pool.GetCount() == 0);
            pool.Spawn();

            Assert.IsTrue(pool.GetCount() == 3);

            var obj = m_MockPoolManager.GetObject<GameObject>("TestPool_01");

            Assert.IsNotNull(obj);

            Assert.IsTrue(pool.GetCount() == 2);

            m_MockPoolManager.Recycle("TestPool_01", obj);

            Assert.IsTrue(pool.GetCount() == 3);
        }

        [Test]
        public void RemovePool()
        {
            Assert.IsTrue(m_MockPoolManager.IsContainsPool("TestPool_01"));

            m_MockPoolManager.RemovePool("TestPool_01");

            Assert.IsFalse(m_MockPoolManager.IsContainsPool("TestPool_01"));
        }
}
}
