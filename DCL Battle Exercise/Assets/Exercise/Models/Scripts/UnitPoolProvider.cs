using System;
using System.Collections.Generic;
using BBB;
using UnityEngine;

[Serializable]
public struct UnitPoolSettings
{
    public UnitType Type;
    public GameObject Prefab;
    public int Quantity;
}
    
public class UnitPoolProvider : MonoBehaviour
{
    [SerializeField] private List<UnitPoolSettings> _prefabs;

    private Dictionary<UnitType, GoPool> _poolsDict;

    public GoPool GetPoolFor(UnitType type)
    {
        if (_poolsDict == null)
        {
            _poolsDict = new Dictionary<UnitType, GoPool>();

            foreach (var prefabPair in _prefabs)
            {
                var goPool = new GoPool(prefabPair.Prefab, transform, prefabPair.Quantity);
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