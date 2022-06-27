
//this interface intended for units and other projects to 
//get access to the surrounding environment

using UnityEngine;

public interface IWorldProxy
{
    
    public Vector3 firstArmyCenter { get; }
    
    public Vector3 secondArmyCenter { get; }
    
    public Vector3 totalArmiesCenter { get; }
}