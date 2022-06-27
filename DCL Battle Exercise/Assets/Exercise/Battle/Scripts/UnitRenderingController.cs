using UnityEngine;

public class UnitRenderingController : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
        
    public void SetColor(Color color)
    {
        _renderer.sharedMaterial.color = color;
    }
}