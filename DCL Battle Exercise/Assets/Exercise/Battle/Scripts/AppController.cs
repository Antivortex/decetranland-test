using System.Collections;
using System.Collections.Generic;
using Exercise.Battle.Scripts;
using Exercise.Models.Scripts;
using UnityEngine;

//battle instantiator renamed to AppController and singleton pattern removed
//since global acceess singletons are evil
public class AppController : MonoBehaviour, IWorldProxy
{

    [SerializeField] private Transform mainCameraTransform;

    private ModelsProvider modelProvider;
    private ObjectPoolProvider objectPoolsProvider;
    private UnitPoolProvider unitPoolProvider;
    private MaterialsProvider materialsProvider;

    [SerializeField] private BoxCollider[] armySpawnColliders;
    [SerializeField] private ArmyModelSO[] armyModels;

    private readonly List<Army> armies = new List<Army>();

    private List<ObjectBase> objects = new List<ObjectBase>();
    private List<ObjectBase> objectsBuffer = new List<ObjectBase>();

    public GameOverMenu gameOverMenu;

    private List<IRenderer> renderers = new List<IRenderer>();
    private List<IRenderer> renderersBuffer = new List<IRenderer>();

    private Vector3 forwardTarget;
    private bool gameOver;
    private GameObject battleField;

    public Vector3 totalArmiesCenter { get; private set; }
    
    void Awake()
    {
        gameOver = false;
        
        LoadManagers();

        var battleFieldPrefab = Resources.Load<GameObject>("Battlefield");
        battleField = GameObject.Instantiate(battleFieldPrefab, parent:null, worldPositionStays:true);
        
        for (int i = 0; i < armySpawnColliders.Length; i++)
        {
            var someCollider = armySpawnColliders[i];
            var army = new Army(i);
            var armyModel = armyModels[i];
            armies.Add(army);
            InstanceArmy(armyModel, army, someCollider.bounds);
        }

        for (int i = 0; i < armies.Count; i++)
        {
            var j = i + 1;
            if (j == armies.Count)
                j = 0;
            
            armies[i].enemyArmy = armies[j];
        }
    }

    private void LoadManagers()
    {
        var modelsProviderPrefab = Resources.Load<GameObject>("ModelsProvider");
        modelProvider = Instantiate(modelsProviderPrefab).GetComponent<ModelsProvider>();

        var objectPoolsProviderPrefab = Resources.Load<GameObject>("ObjectPoolProvider");
        objectPoolsProvider = Instantiate(objectPoolsProviderPrefab).GetComponent<ObjectPoolProvider>();

        var unitPoolProviderPrefab = Resources.Load<GameObject>("UnitPoolProvider");
        unitPoolProvider = Instantiate(unitPoolProviderPrefab).GetComponent<UnitPoolProvider>();

        var materialsProviderPrefab = Resources.Load<GameObject>("MaterialsProvider");
        materialsProvider = Instantiate(materialsProviderPrefab).GetComponent<MaterialsProvider>();
    }

    public IObjectModel GetObjectModel(ObjectType type)
    {
        return modelProvider.GetObjectModel(type);
    }

    public IEnumerable<UnitBase> GetAllUnits()
    {
        foreach(var army in armies)
            foreach (var unit in army.GetUnits())
                yield return unit;
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
        var unitPool = unitPoolProvider.GetPoolFor(unit.Type);
        var unitRenderer = unitPool.Spawn<UnitBaseRenderer>();
        unitRenderer.Init(unit, materialsProvider);
        renderers.Add(unitRenderer);
    }
    
    public void AddObject(ObjectBase obj)
    {
        objects.Add(obj);

        var rendererPool = objectPoolsProvider.GetPoolFor(obj.Type);
        var objRenderer = rendererPool.Spawn<ObjectRendererBase>();
        objRenderer.Init(obj, materialsProvider);
        renderers.Add(objRenderer);
    }

    void UpdateArmyProperties(float deltaTime)
    {

        //here we update army properties separately from units
        //to optimize heavy operation of center calculation
        //to cache it within army object
        totalArmiesCenter = Vector3.zero;

        foreach (var currentArmy in armies)
        {
            currentArmy.Update(deltaTime);
            totalArmiesCenter += currentArmy.center;

            //enemy retargeting for the case of >2 armies
            if (currentArmy.enemyArmy.unitsCount == 0)
            {
                var newEnemy = GetAnyArmyNotDeadExcept(currentArmy);
                if (newEnemy != null)
                    currentArmy.enemyArmy = newEnemy;
            }
        }

        totalArmiesCenter /= armies.Count;
        
        
    }

    private Army GetAnyArmyNotDeadExcept(Army currentArmy)
    {
        foreach (var army in armies)
        {
            if (army.unitsCount == 0)
                continue;

            if (army == currentArmy)
                continue;

            return army;
        }

        return null;
    }

    bool AllOtherArmiesDead(Army army)
    {
        bool result = true;
        foreach (var otherArmy in armies)
        {
            if (army == otherArmy)
                continue;

            result &= otherArmy.unitsCount == 0;
        }

        return result;
    }

    void Update()
    {
        if (gameOver)
            return;
        
        foreach (var army in armies)
        {
            if (AllOtherArmiesDead(army))
            {
                gameOverMenu.gameObject.SetActive(true);
                gameOverMenu.Populate(army);
                gameOver = true;
                return;
            }
        }
        

        var deltaTime = Time.deltaTime;
        UpdateArmyProperties(deltaTime);

        foreach (var army in armies)
        {
            foreach (var unit in army.GetUnits())
            {
                unit.Update(deltaTime, this);
            }
        }
        
        foreach(var obj in objects)
            obj.Update(deltaTime, this);

        foreach (var army in armies)
        {
            army.RemoveDeadUnits();
        }

        RemoveDeadObjects();
        
        UpdateCamera();

        foreach (var r in renderers)
            r.Render(materialsProvider, deltaTime);

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

    private void UpdateCamera()
    {
        Vector3 mainCenter = Vector3.zero;
        foreach (var army in armies)
        {
            mainCenter += army.center;
        }

        mainCenter /= armies.Count;

        forwardTarget = (mainCenter - mainCameraTransform.position).normalized;

        var cameraForward = mainCameraTransform.forward;
        mainCameraTransform.forward += (forwardTarget - cameraForward) * 0.1f;
    }
}