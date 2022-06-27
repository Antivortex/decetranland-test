namespace Exercise.Battle.Scripts
{
    public interface IRenderer
    {
        void Render(MaterialsProvider materialsProvider);
        bool IsDead();
        void Destroy();
    }
}