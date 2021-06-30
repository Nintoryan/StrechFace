using System;
using UnityEngine;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCamTexture;
    private Texture defaultBackGround;
    public RawImage background;
    public AspectRatioFitter fit;

    private void Start()
    {
        defaultBackGround = background.texture;
        var devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("NoCamera");
            camAvailable = false;
            return;
        }
        
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCamTexture = new WebCamTexture(devices[i].name,Screen.width,Screen.height);
            }
        }

        if (backCamTexture == null)
        {
            Debug.Log("No back camera");
            return;
        }
        backCamTexture.Play();
        background.texture = backCamTexture;
        camAvailable = true;
    }

    private void Update()
    {
        throw new NotImplementedException();
    }
}
