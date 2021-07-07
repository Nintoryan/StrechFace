using UnityEngine;

public class ApplyCustomPhoto : MonoBehaviour
{
    public MeshRenderer _streching;
    [SerializeField] private DeviceCamera _deviceCamera;
    [SerializeField] private Canvas _deviceCameraCanvas;
    [SerializeField] private Canvas _gameplayCanvas;

    private void Start()
    {
        _deviceCamera.OnPhotoTaken += ApplyPhoto;
    }

    private void ApplyPhoto(Texture texture,int orient)
    {
        _streching.material.mainTexture = texture;
        _deviceCameraCanvas.gameObject.SetActive(false);
        _gameplayCanvas.gameObject.SetActive(true);

    }
    
}
