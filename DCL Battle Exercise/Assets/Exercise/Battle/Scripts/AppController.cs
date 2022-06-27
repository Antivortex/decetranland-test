using System.Collections.Generic;
using Exercise.Battle.Scripts;
using Exercise.Models.Scripts;
using UnityEngine;

//battle instantiator renamed to AppController and singleton pattern removed
//since global acceess singletons are evil
public class AppController : MonoBehaviour, IWorldProxy
{

    [SerializeField] private Transform mainCameraTransform;

    [SerializeField]
    private ArmyModelSO army1Model;

    [SerializeField]
    private ArmyModelSO army2Model;

    [SerializeField] 
    private ModelsProvider modelProvider;

    [SerializeField] 
    private ObjectPoolProvider _objectPoolsProvider;

    [SerializeField] 
    private UnitPoolProvider _unitPoolProvider;

    [SerializeField]
    private BoxCollider leftArmySpawnBounds;

    [SerializeField]
    private BoxCollider rightArmySpawnBounds;
    

    public readonly Army army1 = new Army();
    public readonly Army army2 = new Army();

    private List<ObjectBase> objects = new List<ObjectBase>();
    private List<ObjectBase> objectsBuffer = new List<ObjectBase>();

    public Color army1Color;
    public Color army2Color;

    public GameOverMenu gameOverMenu;

    private List<IRenderer> renderers = new List<IRenderer>();
    private List<IRenderer> renderersBuffer = new List<IRenderer>();

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

    public IObjectModel GetObjectModel(ObjectType type)
    {
        return modelProvider.GetObjectModel(type);
    }

    void InstanceArmy(IArmyModel armyModel, Army army, Bounds instanceBounds)
    {
        for ( int i = 0; i < armyModel.warriors; i++ )
        {
            var warrior = new Warrior();
            AddUnit(warrior, armyModel, army, instanceBounds);
        }

        for ( int i = 0; i < armyModel.archers; i++ )
        {
            var archer = new Archer();
            AddUnit(archer, armyModel, army, instanceBounds);
        }
    }

    private void AddUnit(UnitBase unit, IArmyModel armyModel, Army army, Bounds instanceBounds)
    {
        var model = modelProvider.GetUnitModel(unit.Type);
        var pos = Utils.GetRandomPosInBounds(instanceBounds);
        unit.Init(model, armyModel, army, pos);
        army.AddUnit(unit);
        var unitPool = _unitPoolProvider.GetPoolFor(unit.Type);
        var unitRenderer = unitPool.Spawn<UnitBaseRenderer>();
        unitRenderer.Init(unit, army.color);
        renderers.Add(unitRenderer);
    }
    
    public void AddObject(ObjectBase obj)
    {
        objects.Add(obj);

        var rendererPool = _objectPoolsProvider.GetPoolFor(obj.Type);
        var objRenderer = rendererPool.Spawn<ObjectRendererBase>();
        renderers.Add(objRenderer);
    }
    
    void Awake()
    {
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
            gameOverMenu.Populate(this);
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
        
        foreach(var obj in objects)
            obj.Update(deltaTime, this);

        army1.RemoveDeadUnits();
        army2.RemoveDeadUnits();

        RemoveDeadObjects();
        
        UpdateCamera(army1, army2);

        foreach (var r in renderers)
            r.Render();

        RemoveDeadRenderers();
    }

    private void RemoveDeadObjects()
    {
        objectsBuffer.Clear();
        foreach (var obj in objects)
        {
            if(!obj.dead)
                objectsBuffer.Add(obj);
        }

        var temp = objects;
        objects = objectsBuffer;
        objectsBuffer = temp;
        
        objectsBuffer.Clear();
    }

    private void RemoveDeadRenderers()
    {
        //double buffer pattern is used here to not allocate lists on every frame
        renderersBuffer.Clear();
        foreach (var r in renderers)
        {
            if (r.IsDead())
            {
                r.Destroy();
            }
            else
            {
                renderersBuffer.Add(r);
            }
        }

        var temp = renderers;
        renderers = renderersBuffer;
        renderersBuffer = temp;
        renderersBuffer.Clear();
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