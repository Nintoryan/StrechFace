using System.Linq;
using UnityEngine;

namespace StcrechingFace.Geometry
{
    public static class Tools
    {
        public static MeshVertex[] GetBrushVertices(Vector3[] meshVertices, Vector3 pointerPosition,float Radius)
        {
            var tmpVertices = new MeshVertex[meshVertices.Length];
            for (int i = 0; i < tmpVertices.Length; i++)
            {
                tmpVertices[i] = new MeshVertex(meshVertices[i],pointerPosition,i);
            }
            return tmpVertices.Where(v => v.distance <= Radius).ToArray();
        }
        public static MeshVertex[] GetBrushVertices(Vector3[] meshVertices, Vector3 pointerPosition,int Amount)
        {
            var tmpVertices = new MeshVertex[meshVertices.Length];
            for (int i = 0; i < tmpVertices.Length; i++)
            {
                tmpVertices[i] = new MeshVertex(meshVertices[i],pointerPosition,i);
            }
            return tmpVertices.OrderBy(v => v.distance).Take(Amount).ToArray();
        }


        public static void AffectMeshVertices(Vector3 difference, MeshVertex[] vertices,float _senDecrCoef = 1f )
        {
            vertices = vertices.OrderBy(v => v.distance).ToArray();
        
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].position += difference * Mathf.Pow(_senDecrCoef,i);
            }
        }
        
        public static void ApplyChangesToMesh(Mesh tmpMesh,MeshFilter _mesh, MeshVertex[] _newVertices)
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
}

