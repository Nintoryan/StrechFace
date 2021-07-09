using System.Collections.Generic;
using Ketchapp.Internal;
using Ketchapp.Internal.Analytics;
using Ketchapp.Internal.Configuration;
using UnityEngine;
#if AppsFlyer
using AppsFlyerSDK;
#endif
namespace Ketchapp.Internal.Analytics
{
    internal class AppsFlyerManager : MonoBehaviour, IAnalyticsManager
    {
        private GameInfos _configuration;

        private string _appsFlyerkey;
        private string _iosAppId;

        public bool IsInitialized { get; set; } = false;

        public void Initialize()
        {
#if AppsFlyer
            _configuration = KetchappInternal.ConfigurationObject;
            if (_configuration)
            {
                _appsFlyerkey = string.IsNullOrEmpty(KetchappInternal.ConfigurationConstants.AppsflyerCustomKey) ? _configuration.GetConfigurationForCurrentPlatform().AppsflyerApiKey : KetchappInternal.ConfigurationConstants.AppsflyerCustomKey;
                _iosAppId = _configuration.GetConfigurationForCurrentPlatform().AppsFlyerAppid;
            }
#if UNITY_IOS
            AppsFlyer.initSDK(_appsFlyerkey, _iosAppId, this);
#endif

#if UNITY_ANDROID
            AppsFlyer.initSDK(_appsFlyerkey, null, this);
#endif
            AppsFlyer.startSDK();
#endif
            IsInitialized = true;
        }

        public void InterstitialShowed(string impressionData)
        {
#if AppsFlyer
            Dictionary<string, string> adEvent = new Dictionary<string, string> { { "af_placement", "INTERSTITIAL" } };
            if (!string.IsNullOrEmpty(impressionData))
            {
                adEvent.Add("af_revenue", impressionData);
            }

            AppsFlyer.sendEvent("af_ads_watched", adEvent);
#endif
        }

        public void RewardVideoValidated(string impressionData)
        {
#if AppsFlyer
            Dictionary<string, string> adEvent = new Dictionary<string, string> { { "af_placement", "REWARD_VIDEO" } };
            if (!string.IsNullOrEmpty(impressionData))
            {
                adEvent.Add("af_revenue", impressionData);
            }

            AppsFlyer.sendEvent("af_ads_watched", adEvent);
#endif
        }

        public void BannerLoaded(string impressionData)
        {
#if AppsFlyer
            Dictionary<string, string> adEvent = new Dictionary<string, string> { { "af_placement", "BANNER" } };
            if (!string.IsNullOrEmpty(impressionData))
            {
                adEvent.Add("af_revenue", impressionData);
            }

            AppsFlyer.sendEvent("af_ads_banner_watched", adEvent);
#endif
        }

        public void InAppPurchaseMade(string item, string currency, string value)
        {
#if AppsFlyer
            Dictionary<string, string> adEvent = new Dictionary<string, string>
        {
            { "af_purchase", $"{item}, {currency}, {value}" }
        };
            AppsFlyer.sendEvent("af_iap_purchase", adEvent);
#endif
        }

        public void ApplicationInstalled()
        {
#if AppsFlyer
            // TODO set params of installation
            Dictionary<string, string> adEvent = new Dictionary<string, string> { { "af_install", string.Empty } };
            AppsFlyer.sendEvent("af_app_install2", adEvent);
#endif
        }

        public void ApplicationSession()
        {
#if AppsFlyer
            // TODO set params of session
            Dictionary<string, string> adEvent = new Dictionary<string, string> { { "af_session", string.Empty } };
            AppsFlyer.sendEvent("af_app_session", adEvent);
#endif
        }

        public void SendAttResult(string eventName)
        {
            Debug.Log("Send ATT result to Appsflyer");
#if AppsFlyer
            Dictionary<string, string> adEvent = new Dictionary<string, string> { { "af_value", eventName } };
            AppsFlyer.sendEvent("af_optin_att2", adEvent);
#endif
        }

        public void LevelStarted(string levelName)
        {
            Debug.Log("Not implemented in AppsFlyer");
        }

        public void LevelPassed(string levelName, int score)
        {
            Debug.Log("Not implemented in AppsFlyer");
        }

        public void LevelFailed(string levelName, int score)
        {
            Debug.Log("Not implemented in AppsFlyer");
        }

        public void CustomEvent(string eventName)
        {
            Debug.Log("Not implemented in AppsFlyer");
        }

        public void CustomEvent(string eventName, float eventValue)
        {
            Debug.Log("Not implemented in AppsFlyer");
        }
    }
}