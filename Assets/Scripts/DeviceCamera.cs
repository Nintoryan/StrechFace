using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DeviceCamera : MonoBehaviour
{
    [SerializeField] private Camera _photoScreenshotTakerCamera;
    
    private bool camAvailable;
    private WebCamTexture CamTexture;
    public RawImage background;
    public RawImage background1;

    public AspectRatioFitter fit;
    public AspectRatioFitter fit1;
    private WebCamDevice FrontalCamera;
    private WebCamDevice BackCamera;
    private WebCamDevice CurrentCamera;
    public event UnityAction<Texture2D,int> OnPhotoTaken; 

    private void OnEnable()
    {
        StartCoroutine(CheckCam());
    }

    private IEnumerator CheckCam()
    {
        var devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            camAvailable = false;
            yield return new WaitForSecondsRealtime(0.2f);
            StartCoroutine(CheckCam());
        }
        else
        {
            if (devices.Any(d => d.isFrontFacing))
            {
                FrontalCamera = devices.First(d => d.isFrontFacing);
                CurrentCamera = FrontalCamera;
                CamTexture = new WebCamTexture(CurrentCamera.name,Screen.width,Screen.height);
                if (CamTexture == null)
                {
                    Debug.Log("No back camera");
                    StartCoroutine(CheckCam());
                }
                else
                {
                    CamTexture.Play();
                    background.texture = CamTexture;
                    background1.texture = CamTexture;
                    camAvailable = true;
                }
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.2f);
                StartCoroutine(CheckCam());
            }
        }
    }
    

    private void OnDisable()
    {
        if (CamTexture != null)
        {
            CamTexture.Stop();
        }
    }

    private int orient;
    private void Update()
    {
        if(!camAvailable) return;
        float ratio = CamTexture.width / (float)CamTexture.height;
        fit.aspectRatio = ratio;
        fit1.aspectRatio = ratio;

        float scaleY = CamTexture.videoVerticallyMirrored ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(1f,scaleY,1f);
        background1.rectTransform.localScale = new Vector3(1f, scaleY,1f);

        orient = -CamTexture.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0,0,orient);
        background1.rectTransform.localEulerAngles = new Vector3(0,0,orient);
    }

    public void TakePhoto()
    {
        StartCoroutine(IETakePhoto());
    }
    private IEnumerator IETakePhoto()
    {
        yield return new WaitForEndOfFrame();
        
        /*var photo = new Texture2D(CamTexture.width,CamTexture.height);
        photo.SetPixels(CamTexture.GetPixels());
        photo.Apply();
        photo = rotateTexture(photo, false);
        photo.Apply();
        photo = FlipTexture(photo);
        photo.Apply();*/
        
        _photoScreenshotTakerCamera.Render();
        var capture = FindObjectOfType<TransparencyImageCapture>();
        
        var imageTuple = capture.captureScreenshot("photo.png", _photoScreenshotTakerCamera);
        var photo = FlipTexture(imageTuple.Item2,false);

        CamTexture.Stop();
        OnPhotoTaken?.Invoke(photo,orient);
    }
    public static Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        var original = originalTexture.GetPixels32();
        var rotated = new Color32[original.Length];
        var w = originalTexture.width;
        var h = originalTexture.height;
        for (var j = 0; j < h; ++j)
        {
            for (var i = 0; i < w; ++i)
            {
                var iRotated = (i + 1) * h - j - 1;
                var iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }
        var rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    Texture2D FlipTexture(Texture2D original, bool upSideDown = true)
    {

        Texture2D flipped = new Texture2D(original.width, original.height);

        int xN = original.width;
        int yN = original.height;


        for (int i = 0; i < xN; i++)
        {
            for (int j = 0; j < yN; j++)
            {
                if (upSideDown)
                {
                    flipped.SetPixel(j, xN - i - 1, original.GetPixel(j, i));
                }
                else
                {
                    flipped.SetPixel(xN - i - 1, j, original.GetPixel(i, j));
                }
            }
        }
        flipped.Apply();

        return flipped;
    }
}
