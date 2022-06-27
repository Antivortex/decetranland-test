
//this interface intended for units and other projects to 
//get access to the surrounding environment

using System.Collections.Generic;
using UnityEngine;

public interface IWorldProxy
{
    public Vector3 totalArmiesCenter { get; }
    void AddObject(ObjectBase arrow);
    IObjectModel GetObjectModel(ObjectType type);
    IEnumerable<UnitBase> GetAllUnits();
}