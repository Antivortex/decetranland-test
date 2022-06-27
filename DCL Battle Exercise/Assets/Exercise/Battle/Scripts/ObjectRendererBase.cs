using BBB;
using Exercise.Battle.Scripts;
using UnityEngine;

public abstract class ObjectRendererBase : MonoBehaviour, IRenderer, IPoolItem
{
    
    [SerializeField] private Renderer _renderer;
    protected Renderer Renderer => _renderer;
    
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

    public void Init(ObjectBase obj, MaterialsProvider materialProvider)
    {
        this.obj = obj;
        SetupMaterials(materialProvider);
    }

    public abstract void SetupMaterials(MaterialsProvider materialProvider);
    public abstract void Render(MaterialsProvider materialsProvider);

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
        _renderer.enabled = false;
    }

    public void OnSpawn()
    {
        _renderer.enabled = true;
    }

    public void OnRelease()
    {
        obj = null;
        _renderer.enabled = false;
    }
}