using System;
using UnityEngine;
using StcrechingFace.End;


public class TransparencyImageCapture:MonoBehaviour
{
    private Texture2D capture(Rect pRect,Camera cam)
    {
        var lCamera = cam;
        Texture2D lOut;
        var lPreClearFlags = lCamera.clearFlags;
        var lPreBackgroundColor = lCamera.backgroundColor;
        {
            lCamera.clearFlags = CameraClearFlags.Color;
            lCamera.Render();
            var lWhiteBackgroundCapture = captureView(pRect);
            lWhiteBackgroundCapture.Apply();
            lOut = lWhiteBackgroundCapture;
        }
        lCamera.backgroundColor = lPreBackgroundColor;
        lCamera.clearFlags = lPreClearFlags;
        return lOut;
    }

    private Texture2D captureScreenshot(Camera cam)
    {
        var rect = new Rect(190f, 325f, 700f, 700f);
        return capture(rect,cam);
    }

    public (Sprite, Texture2D) captureScreenshot(string fileName, Camera cam)
    {
        try
        {
            var SpriteTexture = captureScreenshot(cam);
            var data = SpriteTexture.EncodeToPNG();
            IMG2Sprite.SavePNG(fileName, data);
            return (IMG2Sprite.LoadNewSprite(data), SpriteTexture);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return (null, null);
        }
    }

    private static Texture2D captureView(Rect pRect)
    {
        var lOut = new Texture2D((int)pRect.width, (int)pRect.height, TextureFormat.ARGB32, false);
        lOut.ReadPixels(pRect, 0, 0, false);
        return lOut;
    }

}