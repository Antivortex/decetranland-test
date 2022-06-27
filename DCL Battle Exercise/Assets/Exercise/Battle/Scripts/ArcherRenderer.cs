using UnityEngine;

public class ArcherRenderer : UnitBaseRenderer
{
    //TODO ANTON instantiate arrows from pool
    // public ArcherArrow arrowPrefab;

    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}