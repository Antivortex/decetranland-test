using System;
using System.Collections.Generic;
using BBB;
using UnityEngine;

[Serializable]
public struct ObjectPoolSettings
{
    public ObjectType Type;
    public GameObject Prefab;
    public int Quantity;
}

public class ObjectPoolProvider : MonoBehaviour
{
    [SerializeField] private List<ObjectPoolSettings> _prefabs;
    [SerializeField] private Transform _objectsHolder;

    private Dictionary<ObjectType, GoPool> _poolsDict;

    public GoPool GetPoolFor(ObjectType type)
    {
        if (_poolsDict == null)
        {
            _poolsDict = new Dictionary<ObjectType, GoPool>();

            foreach (var prefabPair in _prefabs)
            {
                var goPool = new GoPool(prefabPair.Prefab, _objectsHolder, prefabPair.Quantity);
                _poolsDict[prefabPair.Type] = goPool;
            }
        }

        if (_poolsDict.TryGetValue(type, out var foundPool))
        {
            return foundPool;
        }
            
        Debug.LogError("Pool not found for " + type);

        return null;
    }
}