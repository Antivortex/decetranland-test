﻿using System.Collections.Generic;
using UnityEngine;

public class Army
{
    //changed to public properties for the sake of incapsulation
    //allows to extend getter and setter with additional lines later if needed
    public Army enemyArmy { get; set; }
    public Color color { get; set; }
    
    public Vector3 center { get; private set; }

    private readonly List<UnitBase> units = new List<UnitBase>();

    public int unitsCount => units.Count;

    public void AddUnit(UnitBase unit)
    {
        units.Add(unit);
    }

    public static IEnumerable<UnitBase> UnitedUnitsFor(params Army[] armies)
    {
        if(armies == null)
            yield break;

        foreach (var army in armies)
        {
            foreach ( var unit in army.units )
            {
                yield return unit;
            }
        }
    }

    //allows to reduce allocations
    public IEnumerable<UnitBase> GetUnits()
    {
        return units;
    }

    public void RemoveUnit(UnitBase unitBase)
    {
        units.Remove(unitBase);
    }

    public void Update(float deltaTime)
    {
        center = Vector3.zero;
        foreach (var unit in units)
        {
            center += unit.position;
        }
        
        center /= units.Count;
    }
}