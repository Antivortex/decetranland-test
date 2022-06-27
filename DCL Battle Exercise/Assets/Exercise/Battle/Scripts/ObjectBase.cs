using UnityEngine;

public abstract class ObjectBase
{
    private static int idCounter;
    
    public static int GetIdentifier()
    {
        return idCounter++;
    }
    
    public int id { get; protected set; }
    public Vector3 position { get; protected set; }
    public Vector3 forward { get; protected set; }
    public bool dead { get; protected set; }
    
    public abstract ObjectType Type { get; }

    public abstract void Update(float deltaTime, IWorldProxy worldProxy);
}