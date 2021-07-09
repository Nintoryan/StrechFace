using UnityEngine;
using System;
#if GameAnalytics
using GameAnalyticsSDK;
#endif
namespace Ketchapp.Internal.Analytics
{
    internal class GameAnalyticsManager : IAnalyticsManager
    {
        public bool IsInitialized { get; set; } = false;

        public void Initialize()
        {
#if GameAnalytics
            if (!IsInitialized)
            {
                GameAnalytics.Initialize();
                IsInitialized = true;
            }
#endif
        }

        public void SendAttResult(string eventValue)
        {
        }

        public void LevelStarted(string levelName)
        {
#if GameAnalytics
            if (IsInitialized)
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelName);
            }
#endif
        }

        public void LevelPassed(string levelName, int score)
        {
#if GameAnalytics
            if (IsInitialized)
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelName, score);
            }
#endif
        }

        public void LevelFailed(string levelName, int score)
        {
#if GameAnalytics
            if (IsInitialized)
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelName, score);
            }
#endif
        }

        public void CustomEvent(string eventName)
        {
#if GameAnalytics
            if (IsInitialized)
            {
                GameAnalytics.NewDesignEvent(eventName);
            }
#endif
        }

        public void CustomEvent(string eventName, float eventValue)
        {
#if GameAnalytics
            if (IsInitialized)
            {
                GameAnalytics.NewDesignEvent(eventName, eventValue);
            }
#endif
        }

        public void InterstitialShowed(string impressionData)
        {
#if GameAnalytics
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "default", "default");
#endif
        }

        public void RewardVideoValidated(string impressionData)
        {
#if GameAnalytics
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "default", "default");
#endif
        }

        public void BannerLoaded(string impressionData)
        {
#if GameAnalytics
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, "default", "default");
#endif
        }

        public void InAppPurchaseMade(string item, string currency, string value)
        {
#if GameAnalytics
            GameAnalytics.NewBusinessEvent(currency, Convert.ToInt32(value), item, "default", "default");
#endif
        }

        public void ApplicationInstalled()
        {
#if GameAnalytics
            var isFirstLaunch = PlayerPrefs.GetInt("FirstLaunch", 1) == 1;
            if (isFirstLaunch)
            {
                GameAnalytics.NewDesignEvent("FirstAppLaunch");
                PlayerPrefs.SetInt("FirstLaunch", 0);
            }
#endif
        }

        public void ApplicationSession()
        {
#if GameAnalytics
            var sessionId = PlayerPrefs.GetInt("SessionCount", 0);
            sessionId++;
            GameAnalytics.NewDesignEvent("AppLaunch", sessionId);
            PlayerPrefs.SetInt("SessionCount", sessionId);
#endif
        }
    }
}