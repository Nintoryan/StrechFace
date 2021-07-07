using UnityEngine;

public class PhotoEndActions : MonoBehaviour
{
    [SerializeField] private ResultSaver _resultSaver;
    private Texture2D _result;

    private void Start()
    {
        _resultSaver.OnSaved += _texture2D =>
        {
            _result = _texture2D;
        };
    }

    public void SavePhotoToGallery()
    {
        NativeGallery.SaveImageToGallery(_result, "Stretched Faces", $"Level {GlobalData.LoadableLevel - 1}.png");
    }

    public void SharePhoto()
    {
        var share = new NativeShare();
        share.SetSubject("Stretched face!");
        share.SetText("Look at this squished face! Ahahhaahhaha xD");
        share.AddFile(_result, $"{Random.Range(1000, 9999)}face.png");
        share.SetTitle("Share this squished face.");
        share.Share();
    }
}
