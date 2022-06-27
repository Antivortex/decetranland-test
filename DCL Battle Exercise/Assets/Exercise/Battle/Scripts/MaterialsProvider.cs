using System.Collections.Generic;
using UnityEngine;

//this is implemented for better materials control
//and also to optimize batching when using
//same materials for multiple objects
public class MaterialsProvider : MonoBehaviour
{
    [SerializeField] private List<Material> _arrowMaterials;
    [SerializeField] private List<Material> _warriorMaterials;
    [SerializeField] private List<Material> _archerMaterials;
    
    public Material GetArrowMaterial(int armyIndex)
    {
        return _arrowMaterials[armyIndex];
    }

    public Material GetWarriorMaterial(int armyIndex)
    {
        return _warriorMaterials[armyIndex];
    }

    public Material GetArcherMaterial(int archerMaterial)
    {
        return _archerMaterials[archerMaterial];
    }
}