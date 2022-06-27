using UnityEngine;

public class ArcherArrow : IHitSource
{
    public float attack { get; private set; }
    public Vector3 position { get; private set; }
}