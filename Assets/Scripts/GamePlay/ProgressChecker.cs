using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ProgressChecker : MonoBehaviour
{
    [SerializeField] private Streching _streching;
    [SerializeField] private MeshFilter _currentMesh;
    [SerializeField] private VertDataAsset _dataAsset;

    private float _startSumDistance;
    private float _currentSumDistance;

    private float _progress;

    public event UnityAction OnLowPass;

    public float Progress
    {
        get => _progress;
        private set
        {
            if (value < 0) return;
            if (value > 0.5f)
            {
                OnLowPass?.Invoke();
            }
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

    private void Awake()
    {
        //_dataAsset.Import();
        _streching.OnChangeVertices += verts=> {Check();};
        _streching.OnOldVerticesApplyed += Check;
        Check();
        _startSumDistance = _currentSumDistance;
    }

    private void Check()
    {
        Progress += 0.05f;
    }
}
