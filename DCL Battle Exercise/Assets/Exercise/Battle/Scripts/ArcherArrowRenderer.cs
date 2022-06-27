using UnityEngine;

public class ArcherArrowRenderer : ObjectRendererBase
{

    public override void SetupMaterials(MaterialsProvider materialProvider)
    {
        if (obj is ArcherArrow arrow)
        {
            Renderer.material = materialProvider.GetArrowMaterial(arrow.army.ArmyIndex);
        }
    }

    public override void Render(MaterialsProvider materialsProvider)
    {
        if (obj is ArcherArrow arrow)
        {
            selfTransform.position = arrow.position;

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