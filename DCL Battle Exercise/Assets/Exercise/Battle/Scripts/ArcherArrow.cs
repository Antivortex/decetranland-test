using System;
using UnityEngine;

public class ArcherArrow : MonoBehaviour
{
    public float speed;

    [NonSerialized] public Vector3 target;
    [NonSerialized] public float attack;

    public Army army;

    public void Update()
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed;
        transform.forward = direction;

        foreach ( var a in army.enemyArmy.GetUnits() )
        {
            float dist = Vector3.Distance(a.transform.position, transform.position);

            if (dist < speed)
            {
                UnitBaseRenderer unit = a.GetComponent<UnitBaseRenderer>();
                unit.Hit(unit);
                Destroy(gameObject);
                return;
            }
        }

        if ( Vector3.Distance(transform.position, target) < speed)
        {
            Destroy(gameObject);
        }
    }
}