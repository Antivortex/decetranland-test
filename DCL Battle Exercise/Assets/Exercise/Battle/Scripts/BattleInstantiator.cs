using System.Collections.Generic;
using UnityEngine;


public class BattleInstantiator : MonoBehaviour, IWorldProxy
{
    public static BattleInstantiator instance { get; private set; }

    [SerializeField] private Transform mainCameraTransform;

    [SerializeField]
    private ArmyModelSO army1Model;

    [SerializeField]
    private ArmyModelSO army2Model;

    [SerializeField] 
    private WarriorModel warriorModel;
    
    [SerializeField] 
    private ArcherModel archerModel;

    [SerializeField]
    private WarriorRenderer warriorPrefab;

    [SerializeField]
    private ArcherRenderer archerPrefab;

    [SerializeField]
    private BoxCollider leftArmySpawnBounds;

    [SerializeField]
    private BoxCollider rightArmySpawnBounds;

    public readonly Army army1 = new Army();
    public readonly Army army2 = new Army();

    public Color army1Color;
    public Color army2Color;

    public GameOverMenu gameOverMenu;

    private readonly List<UnitBaseRenderer> renderers = new List<UnitBaseRenderer>();

    private Vector3 forwardTarget;

    public Vector3 firstArmyCenter { get; set; }
    public Vector3 secondArmyCenter { get; set; }
    public Vector3 GetEnemyArmyCenter(Army ownArmy)
    {
        if (ownArmy == army1)
            return secondArmyCenter;
        else if (ownArmy == army2)
            return firstArmyCenter;

        Debug.LogError("Own army not found");
        return firstArmyCenter;
    }

    public Vector3 totalArmiesCenter { get; set; }


    void InstanceArmy(IArmyModel armyModel, Army army, Bounds instanceBounds)
    {
        for ( int i = 0; i < armyModel.warriors; i++ )
        {
            AddWarrior(armyModel, army, instanceBounds);
        }

        for ( int i = 0; i < armyModel.archers; i++ )
        {
            AddArcher(armyModel, army, instanceBounds);
        }
    }

    private void AddWarrior(IArmyModel armyModel, Army army, Bounds instanceBounds)
    {
        var warrior = new Warrior();
        var pos = Utils.GetRandomPosInBounds(instanceBounds);
        warrior.Init(warriorModel, armyModel, army, pos);
        army.AddUnit(warrior);
        InstantiateRenderer(warrior, warriorPrefab.gameObject, armyModel, army, instanceBounds);
    }

    private void AddArcher(IArmyModel armyModel, Army army, Bounds instanceBounds)
    {
        var archer = new Archer();
        var pos = Utils.GetRandomPosInBounds(instanceBounds);
        archer.Init(warriorModel, armyModel, army, pos);
        army.AddUnit(archer);
        InstantiateRenderer(archer, archerPrefab.gameObject, armyModel, army, instanceBounds);
    }

    private void InstantiateRenderer(UnitBase unitObj, GameObject unitPrefab, IArmyModel model, Army army, Bounds instanceBounds)
    {
        GameObject go = Instantiate(unitPrefab);
        var unitRenderer = go.GetComponent<UnitBaseRenderer>();
        unitRenderer.Init(unitObj, army.color);
        renderers.Add(unitRenderer);
    }

    void Awake()
    {
        instance = this;

        army1.color = army1Color;
        army1.enemyArmy = army2;

        army2.color = army2Color;
        army2.enemyArmy = army1;

        InstanceArmy(army1Model, army1, leftArmySpawnBounds.bounds);
        InstanceArmy(army2Model, army2, rightArmySpawnBounds.bounds);
    }

    void UpdateArmyCenters()
    {

        //here we update army properties separately from units
        //to optimize heavy operation of center calculation
        //to cache it within army object
        firstArmyCenter = Vector3.zero;
        secondArmyCenter = Vector3.zero;
        totalArmiesCenter = Vector3.zero;
        
        foreach (var unit in army1.GetUnits())
        {
            firstArmyCenter += unit.position;
        }

        firstArmyCenter /= army1.unitsCount;

        foreach (var unit in army2.GetUnits())
        {
            secondArmyCenter += unit.position;
        }

        secondArmyCenter /= army2.unitsCount;

        totalArmiesCenter = (firstArmyCenter + secondArmyCenter) * 0.5f;
    }

    void Update()
    {
        if ( army1.unitsCount == 0 || army2.unitsCount == 0 )
        {
            gameOverMenu.gameObject.SetActive(true);
            gameOverMenu.Populate();
            return;
        }
        
        UpdateArmyCenters();

        var deltaTime = Time.deltaTime;
        
        foreach (var unit in army1.GetUnits())
        {
            unit.Update(deltaTime, this);
        }

        foreach (var unit in army2.GetUnits())
        {
            unit.Update(deltaTime, this);
        }

        army1.RemoveDeadUnits();
        army2.RemoveDeadUnits();
        
        UpdateCamera(army1, army2);

        foreach (var r in renderers)
            r.Render();
    }

    private void UpdateCamera(Army army1, Army army2)
    {
        Vector3 mainCenter = army1.center + army2.center;
        mainCenter *= 0.5f;

        forwardTarget = (mainCenter - mainCameraTransform.position).normalized;

        var cameraForward = mainCameraTransform.forward;
        mainCameraTransform.forward += (forwardTarget - cameraForward) * 0.1f;
    }
}