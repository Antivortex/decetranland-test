using System.Collections.Generic;
using UnityEngine;

namespace Exercise.Models.Scripts
{
    public struct UnitPrefabPair
    {
        public UnitType Type;
        public GameObject Prefab;
    }
    
    public class UnitPrefabProvider : MonoBehaviour
    {
        [SerializeField] private List<UnitPrefabPair> _unitPrefabs;

        public GameObject GetPrefab(UnitType type)
        {
            foreach(var pair in _unitPrefabs)
                if (pair.Type == type)
                    return pair.Prefab;

            return null;
        }
    }
}