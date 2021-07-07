using StcrechingFace.End;
using UnityEngine;

public class PhotoChooser : MonoBehaviour
{
    [SerializeField] private MeshRenderer streching;
    [SerializeField] private Canvas PhototypeCanvas;
    [SerializeField] private Canvas GameplayCanvas;
    public void ChoosePhoto()
    {
        var pickCallback = new NativeGallery.MediaPickCallback(path =>
        {
            if (path != null)
            {
                streching.material.mainTexture = DeviceCamera.rotateTexture(IMG2Sprite.LoadTexture(path),false);
                PhototypeCanvas.gameObject.SetActive(false);
                GameplayCanvas.gameObject.SetActive(true);
            }
                
        } );
        NativeGallery.GetImageFromGallery(pickCallback);
    }
}
