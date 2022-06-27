using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase
{
    public Vector3 position { get; private set; }
    public Vector3 forward { get; private set; }
    
    public float health { get; protected set; }
    public float defense { get; protected set; }
    public float attack { get; protected set; }
    public float attackRange { get; protected set; }
    public float maxAttackCooldown { get; protected set; }
    public float postAttackDelay { get; protected set; }
    public float speed { get; protected set; } = 0.1f;

    public IArmyModel armyModel { get; protected set; }
    public Army army { get; protected set; }
    
    protected float attackCooldown;
    
    public bool Dead { get; private set; }
    
    public void Init(IUnitModel unitModel, IArmyModel armyModel, Army army, Vector3 pos)
    {
        health = unitModel.Health;
        defense = unitModel.Defense;
        attack = unitModel.Attack;
        attackRange = unitModel.AttackRange;
        maxAttackCooldown = unitModel.MaxAttackCooldown;
        postAttackDelay = unitModel.PostAttackDelay;
        speed = unitModel.Speed;
        this.armyModel = armyModel;
        this.army = army;
        this.position = pos;
    }

    public virtual void Update(float deltaTime, IWorldProxy worldProxy)
    {
        if ( health < 0 )
            return;

        IEnumerable<UnitBase> allies = army.GetUnits();
        IEnumerable<UnitBase> enemies = army.enemyArmy.GetUnits();

        UpdateBasicRules(allies, enemies, worldProxy);

        switch ( armyModel.strategy )
        {
            case ArmyStrategy.Defensive:
                UpdateDefensive(allies, enemies, worldProxy);
                break;
            case ArmyStrategy.Basic:
                UpdateBasic(allies, enemies, worldProxy);
                break;
        }
    }
    public abstract void Attack(UnitBase enemy);
    protected abstract void UpdateDefensive(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies,
        IWorldProxy worldProxy);
    protected abstract void UpdateBasic(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies,
        IWorldProxy worldProxy);

    public virtual void Move( Vector3 delta )
    {
        if (attackCooldown > maxAttackCooldown - postAttackDelay)
            return;

        position += delta * speed;
    }

    public virtual void Hit( IHitSource source )
    {
        float sourceAttack = source.attack;

        //TODO ANTON handle this
        // if ( source != null )
        // {
        //     sourceAttack = source.attack;
        // }
        // else
        // {
        //     ArcherArrow arrow = source.GetComponent<ArcherArrow>();
        //     sourceAttack = arrow.attack;
        // }
        
        

        health -= Mathf.Max(sourceAttack - defense, 0);

        if ( health < 0 )
        {
            forward = source.position - position;
            Dead = true;

            //TODO ANTON render death
            // var animator = GetComponentInChildren<Animator>();
            // animator?.SetTrigger("Death");
        }
        else
        {
            //TODO ANTON render hit
            // var animator = GetComponentInChildren<Animator>();
            // animator?.SetTrigger("Hit");
        }
    }

    void UpdateBasicRules(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies, IWorldProxy worldProxy)
    {
        attackCooldown -= Time.deltaTime;
        EvadeAllies(allies, worldProxy);
    }

    //in original repo this method is called EvadeAllies but allies argument is not used
    //and evading is implemented for both allies and enemies
    //i left it as is, though seems like it is supposed to be used only for allies
    void EvadeAllies(IEnumerable<UnitBase> allies, IWorldProxy worldProxy)
    {
        Vector3 center = worldProxy.totalArmiesCenter;

        float centerDist = Vector3.Distance(position, center);

        if ( centerDist > 80.0f )
        {
            Vector3 toNearest = (center - position).normalized;
            position -= toNearest * (80.0f - centerDist);
            return;
        }

        foreach ( var unit in army.OwnAndEnemyUnits )
        {
            float dist = Vector3.Distance(position, unit.position);

            if ( dist < 2f )
            {
                Vector3 toNearest = (unit.position - position).normalized;
                position -= toNearest * (2.0f - dist);
            }
        }
    }
}