using UnityEngine;

namespace StcrechingFace.Geometry
{
    public class MeshVertex
    {
        public Vector3 position;
        public readonly int id;
        public readonly float distance;

        public MeshVertex(Vector3 _vertexPosition, Vector3 _pointerPosition, int _id)
        {
            position = _vertexPosition;
            distance = Vector3.Distance(_vertexPosition, _pointerPosition);
            id = _id;
        }
    }
}

