
//this interface intended for units and other projects to 
//get access to the surrounding environment

using Exercise.Models.Scripts;
using UnityEngine;

public interface IWorldProxy
{
    public Vector3 GetEnemyArmyCenter(Army ownArmy);
    
    public Vector3 totalArmiesCenter { get; }
    void AddObject(ObjectBase arrow);
    IObjectModel GetObjectModel(ObjectType type);
}