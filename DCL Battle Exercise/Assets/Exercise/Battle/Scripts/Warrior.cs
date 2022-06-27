using System.Collections.Generic;
using UnityEngine;

public class Warrior : UnitBase, IHitSource
{    
    public Warrior()
    {
        id = ObjectBase.GetIdentifier();
    }
    
    public override UnitType Type => UnitType.Warrior;
    
    public override void Attack( UnitBase targetUnit, IWorldProxy worldProxy )
    {
        if ( attackCooldown > 0 )
            return;

        if ( targetUnit == null )
            return;

        if ( Vector3.SqrMagnitude(position - targetUnit.position) > attackRange*attackRange )
            return;

        attackCooldown = maxAttackCooldown;

        AttacksCount++;

        targetUnit.Hit( this );
    }

    protected override void UpdateDefensive(float deltaTime, IEnumerable<UnitBase> allies,
        IEnumerable<UnitBase> enemies, IWorldProxy worldProxy)
    {
        Vector3 enemyCenter = worldProxy.GetEnemyArmyCenter(army);

        if ( Mathf.Abs( enemyCenter.x - position.x ) > 20 )
        {
            if ( enemyCenter.x < position.x )
                Move( deltaTime, Vector3.left );

            if ( enemyCenter.x > position.x )
                Move( deltaTime, Vector3.right );
        }

        Utils.GetNearestUnit(this, enemies, out UnitBase nearestUnit );

        if ( nearestUnit == null )
            return;

        if ( attackCooldown <= 0 )
            Move(deltaTime, (nearestUnit.position - position).normalized );
        else
        {
            Move(deltaTime, (nearestUnit.position - position).normalized * -1 );
        }

        Attack(nearestUnit, worldProxy);
    }

    protected override void UpdateBasic(float deltaTime, IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies,
        IWorldProxy worldProxy)
    {
        Utils.GetNearestUnit(this, enemies, out UnitBase nearestEnemy );

        if ( nearestEnemy == null )
            return;

        Vector3 toNearest = (nearestEnemy.position - position).normalized;
        toNearest.Scale( new Vector3(1, 0, 1));
        Move(deltaTime, toNearest.normalized );

        Attack(nearestEnemy, worldProxy);
    }
}