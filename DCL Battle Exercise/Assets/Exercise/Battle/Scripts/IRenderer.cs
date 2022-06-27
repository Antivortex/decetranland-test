namespace Exercise.Battle.Scripts
{
    public interface IRenderer
    {
        void Render();
        bool IsDead();
        void Destroy();
    }
}