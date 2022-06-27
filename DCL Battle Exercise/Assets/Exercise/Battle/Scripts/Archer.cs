using System.Collections.Generic;
using UnityEngine;

public class Archer : UnitBase
{
    public override void Attack(UnitBase enemy)
    {
        if ( attackCooldown > 0 )
            return;

        if ( Vector3.Distance(position, enemy.position) > attackRange )
            return;

        attackCooldown = maxAttackCooldown;
        
        //TODO ANTON render attack
        

        // var animator = GetComponentInChildren<Animator>();
        // animator?.SetTrigger("Attack");
        
        //TODO ANTON create arrow here
        // GameObject arrow = Object.Instantiate(arrowPrefab.gameObject);
        // arrow.GetComponent<ArcherArrow>().target = enemy.transform.position;
        // arrow.GetComponent<ArcherArrow>().attack = attack;
        // arrow.GetComponent<ArcherArrow>().army = army;
        // arrow.transform.position = transform.position;

        // if ( army == BattleInstantiator.instance.army1 )
        //     arrow.GetComponent<Renderer>().material.color = BattleInstantiator.instance.army1Color;
        // else
        //     arrow.GetComponent<Renderer>().material.color = BattleInstantiator.instance.army2Color;
    }

   

    protected override void UpdateDefensive(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies,
        IWorldProxy worldProxy)
    {
        Vector3 enemyCenter = Utils.GetCenter(enemies);
        float distToEnemyX = Mathf.Abs( enemyCenter.x - position.x );

        if ( distToEnemyX > attackRange )
        {
            if ( enemyCenter.x < position.x )
                Move( Vector3.left );

            if ( enemyCenter.x > position.x )
                Move( Vector3.right );
        }

        float distToNearest = Utils.GetNearestUnit(this, enemies, out UnitBase nearestEnemy );

        if ( nearestEnemy == null )
            return;

        if ( distToNearest < attackRange )
        {
            Vector3 toNearest = (nearestEnemy.position - position).normalized;
            toNearest.Scale( new Vector3(1, 0, 1));

            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            Move( -(toNearest + flank).normalized );
        }
        else
        {
            Vector3 toNearest = (nearestEnemy.position - position).normalized;
            toNearest.Scale( new Vector3(1, 0, 1));
            Move( toNearest.normalized );
        }

        Attack(nearestEnemy);
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