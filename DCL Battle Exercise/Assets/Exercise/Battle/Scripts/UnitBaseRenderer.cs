using BBB;
using Exercise.Battle.Scripts;
using UnityEngine;

public abstract class UnitBaseRenderer : MonoBehaviour, IRenderer
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;

    protected Renderer Renderer => _renderer;

    private Vector3 _lastPosition;
    private int _lastAttacksCount;
    private int _lastHitsCount;

    private Transform selfTransform;
    protected UnitBase unitToRender;

    //since transform property has significant overhead it makes sense to cache 
    //own transform reference in the base class of the unit
    public Transform SelfTransform
    {
        get
        {
            if (selfTransform == null)
                selfTransform = transform;

            return selfTransform;
        }
    }

    public void Init(UnitBase unit, MaterialsProvider materialsProvider)
    {
        unitToRender = unit;
        SetupMaterials(materialsProvider);
    }

    public abstract void SetupMaterials(MaterialsProvider materialsProvider);
    public void Render(MaterialsProvider materialsProvider, float deltaTime)
    {
        if (unitToRender != null)
        {
            
            
            if (unitToRender.position != _lastPosition)
            {
                if (_animator != null)
                {
                    var delta = (unitToRender.position - _lastPosition).magnitude;
                    _animator.SetFloat("MovementSpeed", delta /
                                                        (unitToRender.speed * deltaTime));
                }
            }
            
            //local position is faster to access then position
            //but it will only work as expected if all of the units
            //are located within same parent or at least parents with 0,0,0 position
            SelfTransform.localPosition = unitToRender.position;
            
            if(unitToRender.forward != Vector3.zero)
                SelfTransform.forward = unitToRender.forward;

            if (unitToRender.AttacksCount > _lastAttacksCount)
            { 
                if(_animator != null)
                    _animator.SetTrigger("Attack");
                _lastAttacksCount = unitToRender.AttacksCount;
            }

            if (unitToRender.HitsCount > _lastHitsCount)
            {
                if(_animator != null)
                    _animator.SetTrigger("Hit");

                _lastHitsCount = unitToRender.HitsCount;
            }

            _lastPosition = unitToRender.position;
        }
    }

    public bool IsDead()
    {
        return unitToRender == null || unitToRender.Dead;
    }

    public virtual void Destroy()
    {
        if(_animator != null)
            _animator.SetTrigger("Death");
        else
        {
            gameObject.Release();
        }
    }
}