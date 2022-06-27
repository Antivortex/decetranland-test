using UnityEngine;

public class ArcherArrow : ObjectBase, IHitSource
{
    public ArcherArrow()
    {
        id = ObjectBase.GetIdentifier();
    }

    public override ObjectType Type => ObjectType.Arrow;
    
    public float attack { get; private set; }
    public float speed { get; private set; }
    public float hitRad { get; private set; }
    public UnitBase target { get; private set; }
    public Army army { get; private set; }
    
    public void Init(Vector3 pos, UnitBase enemyTarget, Army sourceArmy, IWorldProxy worldProxy)
    {
        //this allows to make arrow target following if needed
        position = pos;
        target = enemyTarget;
        army = sourceArmy;
        
        var arrowModel = worldProxy.GetObjectModel(Type) as ArrowModel;
        if (arrowModel != null)
        {
            attack = arrowModel.Attack;
            speed = arrowModel.Speed;
            hitRad = arrowModel.HitRad;
        }
        else
        {
            Debug.LogError("Model not found for arrow");
        }
    }

    public override void Update(float deltaTime, IWorldProxy worldProxy)
    {
        if (dead)
            return;
        
        Vector3 direction = (target.position - position).normalized;
        //i do not like this update since it does not scale with time
        //i keep it as is but i would make it multiplied by time
        //to make speed framerate-independent and also
        //make sure time scale will affect the real time speed
        position += speed * deltaTime * direction;
        forward = direction;

        foreach (var otherUnit in army.enemyArmy.GetUnits())
        {
            float dist = Vector3.Distance(otherUnit.position, position);
            if (dist < hitRad)
            {
                otherUnit.Hit(this);
                dead = true;
                return;
            }
        }
        
        if ( Vector3.Distance(position, target.position) < hitRad)
        {
            dead = true;
            return;
        }

        if (target == null || target.Dead)
        {
            dead = true;
        }
    }



}