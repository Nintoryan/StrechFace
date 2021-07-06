using UnityEngine;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCamTexture;
    public RawImage background;
    public AspectRatioFitter fit;

    private void Start()
    {
        var devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("NoCamera");
            camAvailable = false;
            return;
        }
        backCamTexture = new WebCamTexture(devices[0].name,Screen.width,Screen.height);
        if (backCamTexture == null)
        {
            Debug.Log("No back camera");
            return;
        }
        backCamTexture.Stop();
        //backCamTexture.Play();
        background.texture = backCamTexture;
        camAvailable = true;
    }

    private void Update()
    {
        if(!camAvailable) return;
        float ratio = (float) backCamTexture.width / (float)backCamTexture.height;
        fit.aspectRatio = ratio;

        float scaleY = backCamTexture.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f,scaleY,1f);

        int orient = -backCamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0,0,orient);
    }
}
