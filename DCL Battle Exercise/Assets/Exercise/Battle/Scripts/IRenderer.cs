namespace Exercise.Battle.Scripts
{
    public interface IRenderer
    {
        void Render(MaterialsProvider materialsProvider, float deltaTime);
        bool IsDead();
        void Destroy();
    }
}