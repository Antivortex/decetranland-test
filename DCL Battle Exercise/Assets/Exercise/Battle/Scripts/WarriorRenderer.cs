using System;
using System.Collections.Generic;
using UnityEngine;

public class WarriorRenderer : UnitBaseRenderer
{
    [NonSerialized]
    public float attackRange = 2.5f;

    public override void Attack( UnitBaseRenderer targetUnit )
    {
        if ( attackCooldown > 0 )
            return;

        if ( targetUnit == null )
            return;

        if ( Vector3.SqrMagnitude(SelfTransform.position - targetUnit.SelfTransform.position) > attackRange*attackRange )
            return;

        attackCooldown = maxAttackCooldown;

        animator?.SetTrigger("Attack");

        targetUnit.Hit( targetUnit );
    }

    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }


    protected override void UpdateDefensive(IEnumerable<UnitBaseRenderer> allies, IEnumerable<UnitBaseRenderer> enemies)
    {
        Vector3 enemyCenter = Utils.GetCenter(enemies);

        if ( Mathf.Abs( enemyCenter.x - transform.position.x ) > 20 )
        {
            if ( enemyCenter.x < transform.position.x )
                Move( Vector3.left );

            if ( enemyCenter.x > transform.position.x )
                Move( Vector3.right );
        }

        Utils.GetNearestUnit(this, enemies, out UnitBaseRenderer nearestUnit );

        if ( nearestUnit == null )
            return;

        if ( attackCooldown <= 0 )
            Move( (nearestUnit.SelfTransform.position - SelfTransform.position).normalized );
        else
        {
            Move( (nearestUnit.SelfTransform.position - SelfTransform.position).normalized * -1 );
        }

        Attack(nearestUnit);
    }

    protected override void UpdateBasic(IEnumerable<UnitBaseRenderer> allies, IEnumerable<UnitBaseRenderer> enemies)
    {
        Utils.GetNearestUnit(this, enemies, out UnitBaseRenderer nearestEnemy );

        if ( nearestEnemy == null )
            return;

        Vector3 toNearest = (nearestEnemy.SelfTransform.position - SelfTransform.position).normalized;
        toNearest.Scale( new Vector3(1, 0, 1));
        Move( toNearest.normalized );

        Attack(nearestEnemy);
    }
}