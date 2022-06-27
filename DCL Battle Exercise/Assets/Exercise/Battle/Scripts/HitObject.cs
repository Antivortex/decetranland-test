using UnityEngine;

public interface IHitSource
{
    float attack { get; }
    Vector3 position { get; }
}