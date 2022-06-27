using System.Collections.Generic;
using UnityEngine;

public class Archer : UnitBase
{
    public override void Attack(UnitBaseRenderer enemy)
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateDefensive(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies)
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateBasic(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies)
    {
        throw new System.NotImplementedException();
    }
}