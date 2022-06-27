using BBB;

public class WarriorRenderer : UnitBaseRenderer
{
    public override void SetupMaterials(MaterialsProvider materialProvider)
    {
        if (unitToRender is Warrior w)
        {
            Renderer.material = materialProvider.GetWarriorMaterial(w.army.ArmyIndex);
        }
    }
    
    public void OnDeathAnimFinished()
    {
        gameObject.Release();
    }
}