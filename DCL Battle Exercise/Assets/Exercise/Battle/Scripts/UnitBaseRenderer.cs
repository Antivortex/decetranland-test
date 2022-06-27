using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBaseRenderer : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Animator _animator;

    protected Animator animator => _animator;

    [NonSerialized]
    public IArmyModel armyModel;

    private Vector3 _lastPosition;

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
                _animator.SetFloat("MovementSpeed", (SelfTransform.position - _lastPosition).magnitude / _unitToRender.speed);
            }
            
            SelfTransform.localPosition = _unitToRender.position;
            
            if(_unitToRender.forward != Vector3.zero)
                SelfTransform.forward = _unitToRender.forward;

            _lastPosition = _unitToRender.position;
        }
    }
}