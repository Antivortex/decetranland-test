
public class WarriorRenderer : UnitBaseRenderer
{
    public void OnDeathAnimFinished()
    {
        Destroy(gameObject);
    }
}