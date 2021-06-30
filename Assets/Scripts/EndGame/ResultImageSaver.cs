using UnityEngine;
using UnityEngine.UI;

public class ResultImageSaver : MonoBehaviour
{
    private const int ImageSize = 256;
    [SerializeField] private Camera _saverCamera;
    [SerializeField] private Image _resultImage;
    [SerializeField] private string fileName;
    
    public void SaveFaceSprite()
    {
        var tempRT = new RenderTexture(ImageSize,ImageSize, 24 );

        _saverCamera.targetTexture = tempRT;
        _saverCamera.Render();
     
        RenderTexture.active = tempRT;
        var virtualPhoto = new Texture2D(ImageSize,ImageSize, TextureFormat.ARGB32, false);
        virtualPhoto.ReadPixels( new Rect(0, 0, ImageSize,ImageSize), 0, 0);

        RenderTexture.active = null; 
        _saverCamera.targetTexture = null;
        
        Destroy(tempRT);
        
        var data = virtualPhoto.EncodeToPNG();
        
        Debug.Log(Application.persistentDataPath);
        
        IMG2Sprite.SavePNG(fileName,data);

        _resultImage.sprite = IMG2Sprite.LoadNewSprite(data);
    }

}
