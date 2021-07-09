using System;
using Ketchapp.Internal;
using Ketchapp.Internal.Configuration;
using UnityEngine;

namespace Ketchapp.Internal.Advertisement
{
    internal abstract class AdvertisementBase
    {
        public ConfigType ConfigurationType { get; set; }
        public GameInfos Configuration { get; set; }
        public bool IsInitialized { get; set; }

        public abstract void Initialize();
        public abstract bool IsInterstitialAvailable();
        public abstract void ShowInterstitial(Action<bool> onShown);
        public abstract bool IsRewardedVideoAvailable();
        public abstract void ShowRewardedVideo(Action<bool> onRewardedFinished);
        public abstract void ShowBanner(Action onShown);
        public abstract void HideBanner(Action onHidden = null);
        public abstract void ShowTestSuite();
        public abstract void RefreshUserConsent(bool result);

        public AdvertisementBase()
        {
            Configuration = KetchappInternal.ConfigurationObject;
#if UNITY_IPHONE
            ConfigurationType = ConfigType.Ios;
#elif UNITY_ANDROID
        ConfigurationType = ConfigType.Android;
#endif
        }
    }
}