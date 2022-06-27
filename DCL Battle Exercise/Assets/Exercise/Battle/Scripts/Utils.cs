using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomPosInBounds(Bounds bounds)
    {
        Vector3 pos = Vector3.zero;
        pos.x = Random.Range( bounds.min.x, bounds.max.x );
        pos.z = Random.Range( bounds.min.z, bounds.max.z );
        return pos;
    }
    
    public static Vector3 GetCenter( IEnumerable<UnitBase> units)
    {
        Vector3 result = Vector3.zero;

        int count = 0;    
        foreach ( var unit in units )
        {
            result += unit.position;
            count++;
        }

        result /= count;

        return result;
    }

    public static Vector3 GetCenter( IEnumerable<GameObject> objects)
    {
        Vector3 result = Vector3.zero;

        int count = 0;    
        foreach ( var o in objects )
        {
            result += o.transform.position;
            count++;
        }

        result.x /= count;
        result.y /= count;
        result.z /= count;

        return result;
    }

    public static float GetNearestUnit(UnitBaseRenderer sourceUnit, IEnumerable<UnitBaseRenderer> units, out UnitBaseRenderer nearestUnit)
    {
        float minSqrMagnitude = float.MaxValue;
        nearestUnit = null;

        foreach ( var obj in units )
        {
            //sqr magnitude is better for distance comparison 
            //because it allows to avoid calling sqrt to find real distance
            //but comparison result still holds
            float dist = Vector3.SqrMagnitude(sourceUnit.SelfTransform.position-obj.SelfTransform.position);

            if ( dist < minSqrMagnitude )
            {
                minSqrMagnitude = dist;
                nearestUnit = obj;
            }
        }

        return minSqrMagnitude;
    }

    public static float GetNearestObject( GameObject source, List<GameObject> objects, out GameObject nearestObject )
    {
        float minDist = float.MaxValue;
        nearestObject = null;

        foreach ( var obj in objects )
        {
            float dist = Vector3.Distance(source.transform.position, obj.transform.position);

            if ( dist < minDist )
            {
                minDist = dist;
                nearestObject = obj;
            }
        }

        return minDist;
    }
}