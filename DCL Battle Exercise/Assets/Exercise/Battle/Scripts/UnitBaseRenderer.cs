using BBB;
using Exercise.Battle.Scripts;
using UnityEngine;

public abstract class UnitBaseRenderer : MonoBehaviour, IRenderer
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;

    private Vector3 _lastPosition;
    private int _lastAttacksCount;

    private Transform _selfTransform;
    private UnitBase _unitToRender;

    //since transform property has significant overhead it makes sense to cache 
    //own transform reference in the base class of the unit
    public Transform SelfTransform
    {
        get
        {
            if (_selfTransform == null)
                _selfTransform = transform;

            return _selfTransform;
        }
    }

    public void Init(UnitBase unit, Color color)
    {
        _unitToRender = unit;
        _renderer.sharedMaterial.color = color;
    }

    public void Render()
    {
        if (_unitToRender != null)
        {
            if (_unitToRender.position != _lastPosition)
            {
                if(_animator != null)
                    _animator.SetFloat("MovementSpeed", (SelfTransform.position - _lastPosition).magnitude / _unitToRender.speed);
            }
            
            //local position is faster to access then position
            //but it will only work as expected if all of the units
            //are located within same parent or at least parents with 0,0,0 position
            SelfTransform.localPosition = _unitToRender.position;
            
            if(_unitToRender.forward != Vector3.zero)
                SelfTransform.forward = _unitToRender.forward;

            if (_unitToRender.AttacksCount > _lastAttacksCount)
            { 
                if(_animator != null)
                    _animator.SetTrigger("Attack");
                _lastAttacksCount = _unitToRender.AttacksCount;
            }

            _lastPosition = _unitToRender.position;
        }
    }

    public bool IsDead()
    {
        return _unitToRender == null || _unitToRender.Dead;
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