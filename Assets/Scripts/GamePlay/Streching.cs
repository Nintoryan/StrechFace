using StcrechingFace.Geometry;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Streching : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private MeshFilter _meshFilter;

    [Header("Размер кисти")]

    [SerializeField] private float _brushRadius;

    [SerializeField] private int _brushSize;

    [Header("Чувствительность")] [Range(0.1f, 1f)] [SerializeField]
    private float _senseticity;

    [Header("Коэффициент уменьшения чувствительности")] [Range(0.1f, 1f)] [SerializeField]
    private float _senDecrCoef;
    

    private Camera _camera;
    private Vector3 _dragHitPosition;
    private MeshVertex[] _affectedVertices;

    private Mesh tmpMesh;

    public event UnityAction<Vector3[]> OnChangeVertices;
    public event UnityAction OnOldVerticesApplyed; 
    
    private void Start()
    {
        _camera = Camera.main;
        var sharedMesh = _meshFilter.sharedMesh;
        tmpMesh = new Mesh
        {
            vertices = sharedMesh.vertices,
            triangles = sharedMesh.triangles,
            uv = sharedMesh.uv
        };
        _meshFilter.sharedMesh = tmpMesh;
        OnChangeVertices?.Invoke(tmpMesh.vertices);
    }

    public void ApplyOldVertices(Vector3[] oldVertices)
    {
        tmpMesh.vertices = oldVertices;
        _meshFilter.sharedMesh = tmpMesh;
        OnOldVerticesApplyed?.Invoke();
        
    }

    public void PointerDown(BaseEventData data)
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit)) return;
        
        _dragHitPosition = hit.point;
        _affectedVertices = _brushSize == 0 ? 
            Tools.GetBrushVertices(tmpMesh.vertices, _dragHitPosition, _brushRadius) :
            Tools.GetBrushVertices(tmpMesh.vertices, _dragHitPosition, _brushSize);
    }

    public void PointerDrag(BaseEventData data)
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit)) return;
        
        var pointerDifference = (hit.point - _dragHitPosition)*_senseticity;
        
        Tools.AffectMeshVertices(pointerDifference,_affectedVertices,_senDecrCoef);
        
        Tools.ApplyChangesToMesh(tmpMesh,_meshFilter,_affectedVertices);
        
        _dragHitPosition = hit.point;
    }

    public void PointerUp(BaseEventData data)
    {
        OnChangeVertices?.Invoke(tmpMesh.vertices);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_affectedVertices==null) return;
        Gizmos.color = Color.white;
        foreach (var ver in _affectedVertices)
        {
            Gizmos.DrawSphere(ver.position,0.1f);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_dragHitPosition,_brushRadius);
        
    }
#endif
}
