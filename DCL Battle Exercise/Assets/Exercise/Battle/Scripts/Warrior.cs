using System.Collections.Generic;
using UnityEngine;

public class Warrior : UnitBase, IHitSource
{
    public override void Attack( UnitBase targetUnit )
    {
        if ( attackCooldown > 0 )
            return;

        if ( targetUnit == null )
            return;

        if ( Vector3.SqrMagnitude(position - targetUnit.position) > attackRange*attackRange )
            return;

        attackCooldown = maxAttackCooldown;

        //TODO ANTON animate attack
        // animator?.SetTrigger("Attack");

        targetUnit.Hit( this );
    }

    protected override void UpdateDefensive(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies, IWorldProxy worldProxy)
    {
        Vector3 enemyCenter = worldProxy.GetEnemyArmyCenter(army);

        if ( Mathf.Abs( enemyCenter.x - position.x ) > 20 )
        {
            if ( enemyCenter.x < position.x )
                Move( Vector3.left );

            if ( enemyCenter.x > position.x )
                Move( Vector3.right );
        }

        Utils.GetNearestUnit(this, enemies, out UnitBase nearestUnit );

        if ( nearestUnit == null )
            return;

        if ( attackCooldown <= 0 )
            Move( (nearestUnit.position - position).normalized );
        else
        {
            Move( (nearestUnit.position - position).normalized * -1 );
        }

        Attack(nearestUnit);
    }

    protected override void UpdateBasic(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies,
        IWorldProxy worldProxy)
    {
        Utils.GetNearestUnit(this, enemies, out UnitBase nearestEnemy );

        if ( nearestEnemy == null )
            return;

        Vector3 toNearest = (nearestEnemy.position - position).normalized;
        toNearest.Scale( new Vector3(1, 0, 1));
        Move( toNearest.normalized );

        Attack(nearestEnemy);
    }
}