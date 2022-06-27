using BBB;

public class ArcherRenderer : UnitBaseRenderer
{
    public override void SetupMaterials(MaterialsProvider materialProvider)
    {
        if (unitToRender is Archer a)
        {
            Renderer.material = materialProvider.GetArcherMaterial(a.army.ArmyIndex);
        }
    }
    
    public void OnDeathAnimFinished()
    {
        gameObject.Release();
    }
}