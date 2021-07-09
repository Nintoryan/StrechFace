using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ketchapp.Internal.Configuration
{
    [Serializable]
    public class GameInfos : ScriptableObject
    {
#pragma warning disable SA1401 // Fields should be private
        [SerializeField]
        public string StudioName;
        [SerializeField]
        public string GameName;
        [SerializeField]
        public GameConfiguration IosConfiguration;
        [SerializeField]
        public GameConfiguration AndroidConfiguration;
        [SerializeField]
        public List<string> SKAdNetworksId;
        [SerializeField]
        public List<UnRecommendedVersions> UnRecommendedUnityVersions;
        [SerializeField]
        public float BannerHeight = 180;
        [SerializeField]
        public int GameState;
        public GameConfiguration GetConfigurationForCurrentPlatform()
        {
#if UNITY_IPHONE
        return IosConfiguration;
#elif UNITY_ANDROID
            return AndroidConfiguration;
#else
            return null;
#endif
        }
#pragma warning restore SA1401 // Fields should be private
    }

    public class UnRecommendedVersions
    {
        public string Min { get; set; }
        public string Max { get; set; }
    }

    public enum ConfigType
    {
        Ios,
        Android
    }
}