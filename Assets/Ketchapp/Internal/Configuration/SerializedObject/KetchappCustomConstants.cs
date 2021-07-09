using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class KetchappCustomConstants : ScriptableObject
{
    [SerializeField]
    public int InterstitialDelay = 30;
    [SerializeField]
    public int BannerHeight = 200;
    [SerializeField]
    public string AppsflyerCustomKey;
}

#if UNITY_EDITOR
public static class KetchappCustomConstantsBuilder
{
    public static KetchappCustomConstants Build()
    {
        var constantObject = Resources.Load<KetchappCustomConstants>("KetchappConstants");
        if (constantObject == null)
        {
            var instance = ScriptableObject.CreateInstance(typeof(KetchappCustomConstants));
            AssetDatabase.CreateAsset(instance, Path.Combine("Assets", "Dependencies", "Ketchapp", "Configuration", "Resources", "KetchappConstants.asset"));
            AssetDatabase.Refresh();
            constantObject = (KetchappCustomConstants)instance;
        }

        return constantObject;
    }
}
#endif