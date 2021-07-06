using System;
using UnityEngine;
using System.IO;

namespace StcrechingFace.End
{
    public class IMG2Sprite : MonoBehaviour
    {
        public static Sprite LoadNewSprite(string pFileName, float PixelsPerUnit = 100.0f)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            var path = Application.persistentDataPath + "/" + pFileName;
            if (!File.Exists(path)) return null;
            var SpriteTexture = LoadTexture(path);
            var NewSprite = Sprite.Create(SpriteTexture,
                new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),
                new Vector2(0.5f, 0.5f),
                280f / Mathf.Sqrt(1800000f / (SpriteTexture.width * SpriteTexture.height)),
                1,
                SpriteMeshType.Tight,
                new Vector4(),
                true);
            return NewSprite;
        }

        public static Sprite LoadNewSprite(byte[] _bytes)
        {
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            var SpriteTexture = LoadTexture(_bytes);
            var NewSprite = Sprite.Create(SpriteTexture,
                new Rect(0, 0, SpriteTexture.width, SpriteTexture.height),
                new Vector2(0.5f, 0.5f),
                280f / Mathf.Sqrt(1800000f / (SpriteTexture.width * SpriteTexture.height)),
                1,
                SpriteMeshType.Tight,
                new Vector4(),
                true);
            return NewSprite;
        }

        private static Texture2D LoadTexture(string FilePath)
        {
            if (!File.Exists(FilePath)) return null;
            var FileData = File.ReadAllBytes(FilePath);
            var Tex2D = new Texture2D(2, 2);
            return Tex2D.LoadImage(FileData) ? Tex2D : null;
        }

        private static Texture2D LoadTexture(byte[] FileBytes)
        {
            var Tex2D = new Texture2D(2, 2);
            return Tex2D.LoadImage(FileBytes) ? Tex2D : null;
        }

        public static void SavePNG(string pFileName, byte[] data)
        {
            try
            {
                using var lFile = new FileStream(Application.persistentDataPath + "/" + pFileName, FileMode.Create);
                var lWriter = new BinaryWriter(lFile);
                lWriter.Write(data);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}