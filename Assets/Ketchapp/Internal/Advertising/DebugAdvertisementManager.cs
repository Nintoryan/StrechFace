using System;
using System.Collections;
using Ketchapp.Internal.DebugAds;
using Ketchapp.MayoSDK;
using UnityEngine;

namespace Ketchapp.Internal.Advertisement
{
    internal class DebugAdvertisementManager : AdvertisementBase
    {
        private GameObject DebugInterstitial => Resources.Load<GameObject>("Debug/DebugInterstitial");

        public override void HideBanner(Action onHidden = null)
        {
            KetchappSDK.Advertisement.BannerDisplayed = false;
            onHidden?.Invoke();
        }

        public override void Initialize()
        {
            IsInitialized = true;
            Debug.Log("Debug advertisement manager in use, nothing to initialize");
        }

        public override bool IsInterstitialAvailable()
        {
            return true;
        }

        public override bool IsRewardedVideoAvailable()
        {
            return true;
        }

        public override void ShowBanner(Action onShown)
        {
            KetchappSDK.Advertisement.BannerDisplayed = true;
            onShown?.Invoke();
        }

        public override void ShowInterstitial(Action<bool> onShown)
        {
            var go = GameObject.Instantiate(DebugInterstitial);
            var interstitial = go.GetComponent<DebugInterstitial>();
            interstitial.Initialize(3f, DebugAds.DebugInterstitial.InterstitialDebugType.Interstitial);

            void InterstitialClosed(bool success)
            {
                onShown?.Invoke(success);
                interstitial.OnInterstitialClosed -= InterstitialClosed;
                GameObject.Destroy(go);
            }

            interstitial.OnInterstitialClosed += InterstitialClosed;
        }

        public override void ShowRewardedVideo(Action<bool> onRewardedFinished)
        {
            var go = GameObject.Instantiate(DebugInterstitial);
            var interstitial = go.GetComponent<DebugInterstitial>();
            interstitial.Initialize(7f, DebugAds.DebugInterstitial.InterstitialDebugType.RewardedVideo);

            void InterstitialClosed(bool success)
            {
                onRewardedFinished?.Invoke(success);
                interstitial.OnInterstitialClosed -= InterstitialClosed;
                GameObject.Destroy(go);
            }

            interstitial.OnInterstitialClosed += InterstitialClosed;
        }

        public override void ShowTestSuite()
        {
            Debug.LogWarning("Editor cannot show mediation test");
        }

        public override void RefreshUserConsent(bool result)
        {
            Debug.LogWarning("Mediation GDPR updated, re-fetching Banners/Inters/Rewarded!");
        }
    }
}
