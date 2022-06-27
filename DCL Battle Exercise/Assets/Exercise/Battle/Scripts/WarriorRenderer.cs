using BBB;

public class WarriorRenderer : UnitBaseRenderer
{
    public void OnDeathAnimFinished()
    {
        gameObject.Release();
    }
}