using BBB;

public class ArcherRenderer : UnitBaseRenderer
{
    public void OnDeathAnimFinished()
    {
        gameObject.Release();
    }
}