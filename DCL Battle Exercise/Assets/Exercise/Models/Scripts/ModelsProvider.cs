using System;
using System.Collections.Generic;
using Exercise.Models.Scripts;
using UnityEngine;

[Serializable]
public struct ObjectModelPair
{
    public ObjectType Type;
    public ScriptableObject Model;
}

[Serializable]
public struct UnitModelPair
{
    public UnitType Type;
    public ScriptableObject Model;
}
    
public class ModelsProvider : MonoBehaviour
{
    [SerializeField] private List<UnitModelPair> _unitModels;
    [SerializeField] private List<ObjectModelPair> _objectModels;

    private Dictionary<ObjectType, IObjectModel> _objectModelsDict;
    private Dictionary<UnitType, IUnitModel> _unitModelsDict;

    public IObjectModel GetObjectModel(ObjectType type)
    {
        if (_objectModelsDict == null)
        {
            _objectModelsDict = new Dictionary<ObjectType, IObjectModel>();
            foreach (var pair in _objectModels)
            {
                _objectModelsDict[pair.Type] = pair.Model as IObjectModel;
            }
        }

        if (_objectModelsDict.TryGetValue(type, out var model))
            return model;
            
        Debug.LogError("Model not found for " + type);

        return null;
    }

    public IUnitModel GetUnitModel(UnitType type)
    {
        if (_unitModelsDict == null)
        {
            _unitModelsDict = new Dictionary<UnitType, IUnitModel>();
            foreach (var pair in _unitModels)
            {
                _unitModelsDict[pair.Type] = pair.Model as IUnitModel;
            }
        }

        if (_unitModelsDict.TryGetValue(type, out var model))
            return model;
            
        Debug.LogError("Model not found for " + type);

        return null;
    }
}