using System;
using System.Collections;
using System.Globalization;
using Ketchapp.Internal;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoSDK;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Ketchapp.Internal.Advertisement
{
#if MEDIATION_MAX
    internal class MAXManager : AdvertisementBase, IDisposable
    {
        private string BannerId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.BannerId : KetchappInternal.ConfigurationObject.IosConfiguration.BannerId;
        private string SdkKey => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.MediationAppId : KetchappInternal.ConfigurationObject.IosConfiguration.MediationAppId;
        private string InterstitialId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.InterstitialId : KetchappInternal.ConfigurationObject.IosConfiguration.InterstitialId;
        private string RewardedId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.RewardedVideoId : KetchappInternal.ConfigurationObject.IosConfiguration.RewardedVideoId;

        public override void HideBanner(Action onHidden = null)
        {
            MaxSdk.HideBanner(BannerId);
        }

        public override void Initialize()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += OnSDKInitialized;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += FetchInterstitial;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += FetchRewardedVideo;
            MaxSdkCallbacks.OnBannerAdLoadedEvent += MaxBannerLoaded;
            MaxSdkCallbacks.OnBannerAdLoadFailedEvent += MaxBannerLoadedFailed;
            MaxSdkCallbacks.OnInterstitialDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedDisplayedEvent;

            MaxSdk.SetSdkKey(SdkKey);
            MaxSdk.InitializeSdk();
            MaxSdk.SetHasUserConsent(KetchappInternal.CrossPromo.GetGdprValue());
#if UNITY_IOS && Appsflyer
            MaxSdk.SetUserId(AppsFlyerSDK.AppsFlyeriOS.getAppsFlyerId());
#endif
        }

        private void MaxBannerLoadedFailed(string arg1, int arg2)
        {
            Debug.LogError($"[KetchappMayo MAXManager] MaxBannerLoadedFailed");
        }

        public void MaxBannerLoaded(string placement)
        {
            KetchappInternal.Analytics.BannerLoaded();
        }

        public void Dispose()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent -= OnSDKInitialized;
            MaxSdkCallbacks.OnInterstitialHiddenEvent -= FetchInterstitial;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent -= FetchRewardedVideo;
            MaxSdkCallbacks.OnBannerAdLoadedEvent -= MaxBannerLoaded;
            MaxSdkCallbacks.OnBannerAdLoadFailedEvent -= MaxBannerLoadedFailed;
            MaxSdkCallbacks.OnInterstitialDisplayedEvent -= OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent -= OnRewardedDisplayedEvent;

            GC.SuppressFinalize(this);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId)
        {
            double revenue = MaxSdk.GetAdInfo(adUnitId).Revenue;
            KetchappInternal.Analytics.InterstitialShowed(revenue.ToString(CultureInfo.InvariantCulture));
        }

        private void OnRewardedDisplayedEvent(string adUnitId)
        {
            double revenue = MaxSdk.GetAdInfo(adUnitId).Revenue;
            KetchappInternal.Analytics.RewardVideoValidated(revenue.ToString(CultureInfo.InvariantCulture));
        }

        public override bool IsRewardedVideoAvailable()
        {
            return MaxSdk.IsRewardedAdReady(RewardedId);
        }

        public override void ShowBanner(Action onShown)
        {
            MaxSdk.ShowBanner(BannerId);
            KetchappSDK.Advertisement.BannerDisplayed = true;
            void OnBannerShown(string id)
            {
                onShown?.Invoke();
                MaxSdkCallbacks.OnBannerAdExpandedEvent -= OnBannerShown;
            }

            MaxSdkCallbacks.OnBannerAdExpandedEvent += OnBannerShown;
        }

        public override bool IsInterstitialAvailable()
        {
            return MaxSdk.IsInterstitialReady(InterstitialId);
        }

        public override void ShowInterstitial(Action<bool> onShown)
        {
            if (MaxSdk.IsInterstitialReady(InterstitialId))
            {
                MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += OnInterstitialFail;
                MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialClosed;
                MaxSdk.ShowInterstitial(InterstitialId);

                void OnInterstitialClosed(string id)
                {
                    onShown?.Invoke(true);
                    MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent -= OnInterstitialFail;
                    MaxSdkCallbacks.OnInterstitialHiddenEvent -= OnInterstitialClosed;
                }

                void OnInterstitialFail(string placement, int id)
                {
                    onShown?.Invoke(false);
                    MaxSdkCallbacks.OnInterstitialHiddenEvent -= OnInterstitialClosed;
                    MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent -= OnInterstitialFail;
                }
            }
            else
            {
                Debug.Log("Interstitial is not ready");
                onShown?.Invoke(false);
            }
        }

        public override void ShowRewardedVideo(Action<bool> rewardedFinished)
        {
            if (MaxSdk.IsRewardedAdReady(RewardedId))
            {
                MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += RewardedFinished;
                MaxSdkCallbacks.OnRewardedAdHiddenEvent += RewardedFailed;
                MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += RewardedFailedToDisplay;

                MaxSdk.ShowRewardedAd(RewardedId);
                void ClearCallback()
                {
                    MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent -= RewardedFinished;
                    MaxSdkCallbacks.OnRewardedAdHiddenEvent -= RewardedFailed;
                    MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent -= RewardedFailedToDisplay;
                }

                void RewardedFinished(string id, MaxSdkBase.Reward reward)
                {
                    rewardedFinished?.Invoke(true);
                    ClearCallback();
                }

                void RewardedFailedToDisplay(string id, int error)
                {
                    rewardedFinished?.Invoke(false);
                    ClearCallback();
                }

                void RewardedFailed(string id)
                {
                    rewardedFinished?.Invoke(false);
                    ClearCallback();
                }
            }
        }

        public override void ShowTestSuite()
        {
            MaxSdk.ShowMediationDebugger();
        }

        private void FetchInterstitial(string id)
        {
            MaxSdk.LoadInterstitial(InterstitialId);
        }

        private void FetchRewardedVideo(string id)
        {
            MaxSdk.LoadRewardedAd(RewardedId);
        }

        private void OnSDKInitialized(MaxSdkBase.SdkConfiguration sdkConfiguration)
        {
            IsInitialized = true;
            MaxSdk.CreateBanner(BannerId, MaxSdkBase.BannerPosition.BottomCenter);
            FetchInterstitial(InterstitialId);
            FetchRewardedVideo(RewardedId);
        }

        private void HideBannerBackground(string banner, int id)
        {
            KetchappSDK.Advertisement.BannerDisplayed = false;
        }

        public override void RefreshUserConsent(bool result)
        {
            MaxSdk.SetHasUserConsent(result);
            MaxSdk.CreateBanner(BannerId, MaxSdkBase.BannerPosition.BottomCenter);
            FetchInterstitial(InterstitialId);
            FetchRewardedVideo(RewardedId);

            void OnBannerLoadedAfterRefresh(string id)
            {
                ShowBanner(null);
                MaxSdkCallbacks.OnBannerAdLoadedEvent -= OnBannerLoadedAfterRefresh;
            }

            MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerLoadedAfterRefresh;
        }
    }
#endif
}
