using System.Linq;
using UnityEngine;

public class ProgressChecker : MonoBehaviour
{
    [SerializeField] private Streching _streching;
    [SerializeField] private MeshFilter _currentMesh;
    [SerializeField] private VertDataAsset _dataAsset;

    private float _startSumDistance;
    private float _currentSumDistance;

    private float _progress = 0f;

    public float Progress
    {
        get => _progress;
        private set
        {
            if (value < 0) return;
            if (value > 1f)
            {
                _progress = 1;
            }
            else
            {
                _progress = value;
            }

        }
    }

    private void Start()
    {
        Check();
        _dataAsset.Import();
        _streching.OnPointerUP += Check;
        _startSumDistance = _currentSumDistance;
    }

    private void Check()
    {
        _currentSumDistance = _currentMesh.sharedMesh.vertices.
            Select((t, i) => Vector3.Distance(t, _dataAsset.data.Vertecies[i])).Sum();
        Progress = 2.5f * (1 - _currentSumDistance / _startSumDistance);
        Debug.Log(Progress);
    }
}
