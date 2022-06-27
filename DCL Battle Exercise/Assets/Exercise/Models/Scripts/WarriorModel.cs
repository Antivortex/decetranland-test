using UnityEngine;


/// <summary>
/// ScriptableObject containing the data of an army
/// for simplicity's sake the use-case of updating the SO manually has been discarded, and
/// therefore the usage of ReadOnlyAttribute
/// </summary>
[CreateAssetMenu(menuName = "Create WarriorModel", fileName = "WarriorModel", order = 0)]
public class WarriorModel : ScriptableObject, IUnitModel
{
    [ReadOnlyAttribute, SerializeField] private float health = 50f;
    [ReadOnlyAttribute, SerializeField] private float defense = 5f;
    [ReadOnlyAttribute, SerializeField] private float attack = 20f;
    [ReadOnlyAttribute, SerializeField] private float attackRange = 2.5f;
    [ReadOnlyAttribute, SerializeField] private float maxAttackCooldown = 1f;
    [ReadOnlyAttribute, SerializeField] private float postAttackDelay = 0f;
    [ReadOnlyAttribute, SerializeField] private float speed = 0.1f;

    public float Health => health;
    public float Defense => defense;
    public float Attack => attack;
    public float AttackRange => attackRange;
    public float MaxAttackCooldown => maxAttackCooldown;
    public float PostAttackDelay => postAttackDelay;
    public float Speed => speed;
}