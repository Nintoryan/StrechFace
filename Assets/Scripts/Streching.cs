using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Streching : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [Header("Размер кисти")]
    [SerializeField] private int _brushSize;

    [Header("Чувствительность")] [Range(0.1f, 1f)] [SerializeField]
    private float _senseticity;
    

    private Camera _camera;
    private Vector3 _dragHitPosition;
    private MeshVertex[] _affectedVertices;

    private Mesh tmpMesh;

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
    }

    public void PointerDown(BaseEventData data)
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            _dragHitPosition = hit.point;
            Debug.Log($"downHitPosition {_dragHitPosition}");
            _affectedVertices = GetSortedVertices(tmpMesh.vertices, _dragHitPosition, _brushSize);
            foreach (var affectedVertex in _affectedVertices)
            {
                Debug.Log($"position {affectedVertex.position} distance{affectedVertex.distance} id {affectedVertex.id}");
            }
        }

    }

    public void PointerDrag(BaseEventData data)
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            var pointerDifference = (hit.point - _dragHitPosition)*_senseticity;
            AffectMeshVertices(pointerDifference,_affectedVertices);
            ApplyChangesToMesh(_meshFilter,_affectedVertices);
            _dragHitPosition = hit.point;
        }
        
    }

    private MeshVertex[] GetSortedVertices(Vector3[] meshVertices, Vector3 pointerPosition, int Amount)
    {
        var tmpVertices = new MeshVertex[meshVertices.Length];
        for (int i = 0; i < tmpVertices.Length; i++)
        {
            tmpVertices[i] = new MeshVertex(meshVertices[i],pointerPosition,i);
        }

        return tmpVertices.OrderBy(v => v.distance).Take(Amount).ToArray();
    }


    private void AffectMeshVertices(Vector3 difference, IEnumerable<MeshVertex> vertices)
    {
        foreach (var vertex in vertices)
        {
            vertex.position += difference;
        }
    }

    private void OnDrawGizmos()
    {
        if(_affectedVertices==null) return;
        Gizmos.color = Color.white;
        foreach (var ver in _affectedVertices)
        {
            Gizmos.DrawSphere(ver.position,0.1f);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(_dragHitPosition,0.2f);
        
    }

    private void ApplyChangesToMesh(MeshFilter _mesh, MeshVertex[] _newVertices)
    {
        var _meshVertices = _mesh.sharedMesh.vertices;
        foreach (var newVertex in _newVertices)
        {
            _meshVertices[newVertex.id] = newVertex.position;
        }
        tmpMesh.vertices = _meshVertices;
        _mesh.sharedMesh = tmpMesh;
    }
}

public class MeshVertex
{
    public Vector3 position;
    public int id;
    public float distance;

    public MeshVertex(Vector3 _vertexPosition, Vector3 _pointerPosition, int _id)
    {
        position = _vertexPosition;
        distance = Vector3.Distance(_vertexPosition, _pointerPosition);
        id = _id;
    }
}
