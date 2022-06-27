using UnityEngine;


/// <summary>
/// ScriptableObject containing the data of an army
/// for simplicity's sake the use-case of updating the SO manually has been discarded, and
/// therefore the usage of ReadOnlyAttribute
/// </summary>
[CreateAssetMenu(menuName = "Create ArrowModel", fileName = "ArrowModel", order = 0)]
public class ArrowModel : ScriptableObject, IObjectModel
{
    [SerializeField] private float attack = 5f;
    [SerializeField] private float speed = 0.04f;
    [SerializeField] private float hitRad = 2.5f;

    public float Attack => attack;
    public float Speed => speed;
    public float HitRad => hitRad;
}