using System.Collections.Generic;
using UnityEngine;

public class Archer : UnitBase
{
    public Archer()
    {
        id = ObjectBase.GetIdentifier();
    }
    
    public override UnitType Type => UnitType.Archer;
    
    public override void Attack(UnitBase enemy, IWorldProxy worldProxy)
    {
        if ( attackCooldown > 0 )
            return;

        if ( Vector3.Distance(position, enemy.position) > attackRange )
            return;

        attackCooldown = maxAttackCooldown;

        AttacksCount++;

        var arrow = new ArcherArrow();
        arrow.Init(pos:position, enemyTarget:enemy, sourceArmy:army, worldProxy);
        worldProxy.AddObject(arrow);
    }

   

    protected override void UpdateDefensive(float deltaTime, IEnumerable<UnitBase> allies,
        IEnumerable<UnitBase> enemies,
        IWorldProxy worldProxy)
    {
        Vector3 enemyCenter = army.enemyArmy.center;
        float distToEnemyX = Mathf.Abs( enemyCenter.x - position.x );

        if ( distToEnemyX > attackRange )
        {
            if ( enemyCenter.x < position.x )
                Move(deltaTime, Vector3.left );

            if ( enemyCenter.x > position.x )
                Move(deltaTime, Vector3.right );
        }

        float distToNearest = Utils.GetNearestUnit(this, enemies, out UnitBase nearestEnemy );

        if ( nearestEnemy == null )
            return;

        if ( distToNearest < attackRange )
        {
            Vector3 toNearest = (nearestEnemy.position - position).normalized;
            toNearest.Scale( new Vector3(1, 0, 1));

            Vector3 flank = Quaternion.Euler(0, 90, 0) * toNearest;
            Move(deltaTime, -(toNearest + flank).normalized );
        }
        else
        {
            Vector3 toNearest = (nearestEnemy.position - position).normalized;
            toNearest.Scale( new Vector3(1, 0, 1));
            Move( deltaTime, toNearest.normalized );
        }

        Attack(nearestEnemy, worldProxy);
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