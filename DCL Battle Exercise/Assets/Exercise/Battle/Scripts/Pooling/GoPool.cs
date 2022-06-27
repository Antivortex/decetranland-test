using System;
using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace BBB
{
    public interface IPoolItemReleaseNotifier
    {
        event Action OnReleaseEvent;
    }

    public interface IPoolItem
    {
        void OnInstantiate();
        void OnSpawn();
        void OnRelease();
    }

    public interface IGoPool
    {
        void Release(GameObject go);
    }

    public class PoolItem : MonoBehaviour
    {
        public IGoPool Pool;
    }

    public class GoPool : IGoPool
    {
        private GameObject _prefab;
        private readonly Transform _parent;
        private readonly Action<GameObject> _onCreate;
        private readonly Stack<GameObject> _pool;

        /// <summary>
        /// sole purpose of this collection is to contain exactly the same content as Stack pool
        /// and serve as a duplicate filter on releasing an object back to the pool
        /// </summary>
        private readonly HashSet<GameObject> _poolSet;

        private readonly HashSet<GameObject> _spawnedObjectsSet;

        /// <summary>
        /// Holds a bool flag that indicates is the prototype for instantiation contains the IPooled component.
        /// Used to skip GetComponent call if it is not needed. -VK 
        /// </summary>
        private bool? _hasPooledComponentInPrototype = null;

        public GameObject Prefab
        {
            get => _prefab;
            protected set => _prefab = value;
        }

        public GoPool(GameObject prefab, Transform parent, int quantity, Action<GameObject> onCreate = null)
        {
            _prefab = prefab;
            _parent = parent;
            _onCreate = onCreate;
            _pool = new Stack<GameObject>(quantity);
            _poolSet = new HashSet<GameObject>();
            _spawnedObjectsSet = new HashSet<GameObject>();
            Prewarm(quantity);
        }

        public GameObject Spawn()
        {
            var go = _pool.Count <= 0 ? Create() : _pool.Pop();
            _poolSet.Remove(go);
            _spawnedObjectsSet.Add(go);
            TriggerOnSpawnForPopItem(go);
            return go;
        }

        public TComponent Spawn<TComponent>() where TComponent : Component
        {
            var go = _pool.Count <= 0 ? Create() : _pool.Pop();
            _poolSet.Remove(go);
            _spawnedObjectsSet.Add(go);
            TriggerOnSpawnForPopItem(go);

            var result =  go.GetComponent<TComponent>();
            if (result == null)
            {
                Debug.LogError($"Spawned item '{go.name}' doesn't have target component: {nameof(TComponent)}", go);
                return null;
            }

            return result;
        }

        private void TriggerOnSpawnForPopItem(GameObject item)
        {
            if (_hasPooledComponentInPrototype.HasValue)
            {
                if (_hasPooledComponentInPrototype.Value)
                {
                    var pooledItem = item.GetComponent<IPoolItem>();
                    pooledItem.OnSpawn();
                }
            }
            else
            {
                var pooledItem = item.GetComponent<IPoolItem>();
                if (pooledItem == null)
                {
                    _hasPooledComponentInPrototype = false;
                }
                else
                {
                    _hasPooledComponentInPrototype = true;
                    pooledItem.OnSpawn();
                }
            }
        }

        public void Release(GameObject go)
        {
            //if the reference to this gameobject is already in the pool,
            //we should not allow it being released for the second time
            //this check guarantees double call to Release will not lead to
            //same object return from Spawn twice in a row
            if (_poolSet.Contains(go))
                return;
        
            _pool.Push(go);

            _poolSet.Add(go);
            _spawnedObjectsSet.Remove(go);
            var poolItem = go.GetComponent<IPoolItem>();
            if (poolItem != null)
            {
                poolItem.OnRelease();
            }
        }

        private static readonly List<GameObject> TempGoList = new List<GameObject>();

        public void ForceReleaseAll()
        {
            TempGoList.Clear();
            foreach (var go in _spawnedObjectsSet)
            {
                TempGoList.Add(go);
            }

            foreach (var go in TempGoList)
            {
                go.Release();
            }
            
            TempGoList.Clear();
        }

        public void Cleanup()
        {
            while (_pool.Count > 0)
            {
                var go = _pool.Pop();
                _poolSet.Remove(go);
                go.SetActive(false);
                Object.Destroy(go);
            }
            
            _pool.Clear();
            _poolSet.Clear();
        }

        private GameObject Create()
        {
            UnityEngine.Profiling.Profiler.BeginSample($"Instantiate[{_prefab.name}]");
            var go = Object.Instantiate(_prefab, _parent, false);
            UnityEngine.Profiling.Profiler.EndSample();
            var item = go.AddComponent<PoolItem>();
            
            if(_onCreate != null)
                _onCreate(go);

            item.Pool = this;

            var poolItem = go.GetComponent<IPoolItem>();
            if (poolItem != null)
            {
                poolItem.OnInstantiate();
            }

            return go;
        }

        private void Prewarm(int quantity)
        {
            for (var i = 0; i < quantity; i++)
            {
                var go = Create();
                _pool.Push(go);
                _poolSet.Add(go);
            }
        }
    }

    public static class RenderPoolExtensions
    {
        public static void Release(this GameObject objectToDestroy)
        {
            var poolItem = objectToDestroy.GetComponent<PoolItem>();
            if (poolItem != null)
            {
                poolItem.Pool.Release(objectToDestroy);
            }
            else
            {
                Debug.LogError($"Trying to release GameObject as pooled item, but it was created not in pool. '{objectToDestroy.name}'");
                Object.Destroy(objectToDestroy);
            }
        }
    }
}
