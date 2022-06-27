using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase
{
    public Vector3 position { get; private set; }
    
    public float health { get; protected set; }
    public float defense { get; protected set; }
    public float attack { get; protected set; }
    public float maxAttackCooldown { get; protected set; }
    public float postAttackDelay { get; protected set; }
    public float speed { get; protected set; } = 0.1f;

    public IArmyModel armyModel { get; protected set; }
    public Army army { get; protected set; }
    
    protected float attackCooldown;
    
    private Vector3 _lastPosition;
    
    public void Init(IUnitModel unitModel, IArmyModel armyModel, Army army, Vector3 pos)
    {
        health = unitModel.Health;
        defense = unitModel.Defense;
        attack = unitModel.Attack;
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

        UpdateBasicRules(allies, enemies);

        switch ( armyModel.strategy )
        {
            case ArmyStrategy.Defensive:
                UpdateDefensive(allies, enemies);
                break;
            case ArmyStrategy.Basic:
                UpdateBasic(allies, enemies);
                break;
        }
       
        _lastPosition = position;
    }
    public abstract void Attack(UnitBaseRenderer enemy);
    protected abstract void UpdateDefensive(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies);
    protected abstract void UpdateBasic(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies);

    public virtual void Move( Vector3 delta )
    {
        if (attackCooldown > maxAttackCooldown - postAttackDelay)
            return;

        position += delta * speed;
    }

    public virtual void Hit( UnitBase source )
    {
        float sourceAttack = 0;

        if ( source != null )
        {
            sourceAttack = source.attack;
        }
        else
        {
            ArcherArrow arrow = source.GetComponent<ArcherArrow>();
            sourceAttack = arrow.attack;
        }

        health -= Mathf.Max(sourceAttack - defense, 0);

        if ( health < 0 )
        {
            SelfTransform.forward = source.SelfTransform.position - SelfTransform.position;
            army.RemoveUnit(this);

            var animator = GetComponentInChildren<Animator>();
            animator?.SetTrigger("Death");
        }
        else
        {
            var animator = GetComponentInChildren<Animator>();
            animator?.SetTrigger("Hit");
        }
    }

    void UpdateBasicRules(IEnumerable<UnitBase> allies, IEnumerable<UnitBase> enemies)
    {
        attackCooldown -= Time.deltaTime;
        EvadeAllies(allies);
    }

    void EvadeAllies(IEnumerable<UnitBaseRenderer> allies)
    {
        var allUnits = Army.UnitedUnitsFor(army, army.enemyArmy);

        Vector3 center = Utils.GetCenter(allUnits);
        
        var selfPosition = SelfTransform.position;

        float centerDist = Vector3.Distance(selfPosition, center);


        if ( centerDist > 80.0f )
        {
            Vector3 toNearest = (center - SelfTransform.position).normalized;
            SelfTransform.position = selfPosition - toNearest * (80.0f - centerDist);
            return;
        }

        foreach ( var unit in allUnits )
        {
            float dist = Vector3.Distance(SelfTransform.position, unit.SelfTransform.position);

            if ( dist < 2f )
            {
                Vector3 toNearest = (unit.SelfTransform.position - SelfTransform.position).normalized;
                SelfTransform.position = selfPosition - toNearest * (2.0f - dist);
            }
        }
    }
}