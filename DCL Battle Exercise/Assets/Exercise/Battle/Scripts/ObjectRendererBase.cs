using BBB;
using Exercise.Battle.Scripts;
using UnityEngine;

public abstract class ObjectRendererBase : MonoBehaviour, IRenderer, IPoolItem
{
    protected ObjectBase obj;

    private Transform _selfTransform;
    protected Transform selfTransform
    {
        get
        {
            if (_selfTransform == null)
            {
                _selfTransform = transform;
            }

            return _selfTransform;
        }
    }

    public void Init(ObjectBase obj)
    {
        this.obj = obj;
    }
    
    public abstract void Render();

    public bool IsDead()
    {
        return obj == null || obj.dead;
    }

    public virtual void Destroy()
    {
        gameObject.Release();
    }

    //pool object API
    public void OnInstantiate()
    {
        
    }

    public void OnSpawn()
    {
        
    }

    public void OnRelease()
    {
        obj = null;
    }
}