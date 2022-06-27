using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Army
{
    //changed to public properties for the sake of incapsulation
    //allows to extend getter and setter with additional lines later if needed
    public Army enemyArmy { get; set; }
    public Color color { get; set; }
    
    public Vector3 center { get; private set; }

    private List<UnitBase> units = new List<UnitBase>();
    private List<UnitBase> unitsBuffer = new List<UnitBase>();

    public IEnumerable<UnitBase> OwnAndEnemyUnits
    {
        get
        {
            foreach (var unit in units)
                yield return unit;

            foreach (var unit in enemyArmy.units)
                yield return unit;
        }
    }

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

    public void RemoveDeadUnits()
    {
        unitsBuffer.Clear();
        foreach (var unit in units)
        {
            if(!unit.Dead)
                unitsBuffer.Add(unit);
        }

        var temp = units;
        units = unitsBuffer;
        unitsBuffer = temp;
        unitsBuffer.Clear();

    }
}