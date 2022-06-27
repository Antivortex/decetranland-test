public interface IUnitModel
{
    public float Health { get; }
    public float Defense { get; }
    public float Attack { get; }
    public float AttackRange{ get; }
    public float MaxAttackCooldown { get; }
    public float PostAttackDelay{ get; }
    public float Speed { get; }
}