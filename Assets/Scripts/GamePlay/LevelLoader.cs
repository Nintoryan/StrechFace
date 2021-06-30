using MPUIKIT;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private MPImage[] BackGrounds;
    [SerializeField] private Material[] Outlines;
    [SerializeField] private MeshRenderer _outline;

    private void Awake()
    {
        _outline.material = Outlines[GlobalData.LoadableLevel % Outlines.Length];
        BackGrounds[GlobalData.LoadableLevel % BackGrounds.Length].gameObject.SetActive(true);
    }
    
}
