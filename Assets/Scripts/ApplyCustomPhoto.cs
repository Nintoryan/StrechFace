using UnityEngine;

public class ApplyCustomPhoto : MonoBehaviour
{
    public MeshRenderer _streching;
    [SerializeField] private DeviceCamera _deviceCamera;
    [SerializeField] private Canvas _deviceCameraCanvas;
    [SerializeField] private Canvas _gameplayCanvas;
    public void TakePhoto()
    {
        _streching.material.mainTexture = _deviceCamera.background.texture;
        _deviceCameraCanvas.gameObject.SetActive(false);
        _gameplayCanvas.gameObject.SetActive(true);
    }
}
