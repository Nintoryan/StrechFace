using System;
using System.Collections.Generic;
using System.Linq;
#if FacebookSDK
using Facebook.Unity;
#endif
using UnityEngine;

namespace Ketchapp.Internal.Analytics
{
    internal class AnalyticsManager : IDisposable
    {
        private AppsFlyerManager _appsFlyerManager;
        private GameAnalyticsManager _gameAnalyticsManager;

        private string AppInstallEventKey => "AppInstalledEventFired";
        private List<IAnalyticsManager> _analyticsManagers = new List<IAnalyticsManager>();

        public AnalyticsManager()
        {
#if GameAnalytics
            _analyticsManagers.Add(new GameAnalyticsManager());
#endif
        }

        public void Initialize()
        {
#if AppsFlyer
            _analyticsManagers.Add(GameObject.Find("Appsflyer").GetComponent<IAnalyticsManager>());
#endif
            _analyticsManagers.ForEach(m => m.Initialize());
            if (!AppInstalledFired())
            {
                _analyticsManagers.ForEach(m => m.ApplicationInstalled());
                PlayerPrefs.SetInt(AppInstallEventKey, 1);
            }

            _analyticsManagers.ForEach(m => m.ApplicationSession());

#if FacebookSDK
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                FB.Init(() =>
                {
                    if (FB.IsInitialized)
                    {
                        FB.ActivateApp();
                    }
                    else
                    {
                        Debug.Log("Failed to initialize facebook sdk");
                    }
                });
            }
#endif
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void InterstitialShowed(string impressionData = "")
        {
            _analyticsManagers.ForEach(m => m.InterstitialShowed(impressionData));
        }

        public void RewardVideoValidated(string impressionData = "")
        {
            _analyticsManagers.ForEach(m => m.RewardVideoValidated(impressionData));
        }

        public void BannerLoaded(string impressionData = "")
        {
            _analyticsManagers.ForEach(m => m.BannerLoaded(impressionData));
        }

        /// <summary>
        /// TODO make a link with IAP module.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="currency"></param>
        /// <param name="value"></param>
        public void InAppPurchaseMade(string item, string currency, string value)
        {
            _analyticsManagers.ForEach(m => m.InAppPurchaseMade(item, currency, value));
        }

        public void ApplicationInstalled()
        {
            _analyticsManagers.ForEach(m => m.ApplicationInstalled());
        }

        public void ApplicationSession()
        {
            _analyticsManagers.ForEach(m => m.ApplicationSession());
        }

        public void SendAttResult(string eventName)
        {
            _analyticsManagers.ForEach(m => m.SendAttResult(eventName));
        }

        public void LevelStarted(string levelName)
        {
            _analyticsManagers.ForEach(m => m.LevelStarted(levelName));
        }

        public void LevelPassed(string levelName, int score)
        {
            _analyticsManagers.ForEach(m => m.LevelPassed(levelName, score));
        }

        public void LevelFailed(string levelName, int score)
        {
            _analyticsManagers.ForEach(m => m.LevelFailed(levelName, score));
        }

        public void CustomEvent(string eventName)
        {
            _analyticsManagers.ForEach(m => m.CustomEvent(eventName));
        }

        public void CustomEvent(string eventName, float eventValue)
        {
            _analyticsManagers.ForEach(m => m.CustomEvent(eventName, eventValue));
        }

        private bool AppInstalledFired()
        {
            return PlayerPrefs.GetInt(AppInstallEventKey, 0) == 1;
        }
    }
}
