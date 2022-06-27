using UnityEngine;

public class ArcherArrowRenderer : ObjectRendererBase
{
    [SerializeField] private Renderer _renderer;

    public override void Render()
    {
        if (obj is ArcherArrow arrow)
        {
            selfTransform.position = arrow.position;
            _renderer.sharedMaterial.color = arrow.army.color;

            if (arrow.forward != Vector3.zero)
            {
                selfTransform.forward = arrow.forward;
            }
        }
        else
        {
            Debug.LogError("Wrong type of object: " + obj?.GetType());
        }
    }
}