using MPUIKIT;
using UnityEngine;
using UnityEngine.Events;

public class SelectableFace : MonoBehaviour
{
    [SerializeField] private MeshRenderer _strechingMeshRenderer;
    [SerializeField] private Material _material;
    [SerializeField] private MPImage _backGround;
    [Header("Colors")] 
    [SerializeField] private Color SelectedColor;
    [SerializeField] private Color AvailableColor;

    private static event UnityAction OnSelected;

    private void Awake()
    {
        OnSelected += Deselect;
    }

    private void Deselect()
    {
        if(_backGround == null) return;
        _backGround.color = AvailableColor;
    }
    
    public void Select()
    {
        OnSelected?.Invoke();
        
        _backGround.color = SelectedColor;
        _strechingMeshRenderer.material = _material;
    }
}
