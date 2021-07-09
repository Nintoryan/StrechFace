using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Ketchapp.Internal;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoSDK;
using UnityEngine;
namespace Ketchapp.Internal.Advertisement
{
#if MEDIATION_IronSource
    internal class IronSourceManager : AdvertisementBase, IDisposable
    {
        private string AppId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.MediationAppId : KetchappInternal.ConfigurationObject.IosConfiguration.MediationAppId;

        public IronSourceManager()
        {
        }

        public override void HideBanner(Action onHidden)
        {
            IronSource.Agent.hideBanner();
        }

        public override void Initialize()
        {
            IronSource.Agent.init(AppId);

            IronSourceEvents.onInterstitialAdClosedEvent += IronSource.Agent.loadInterstitial;

            IronSource.Agent.setConsent(KetchappSDK.CrossPromo.GetGdprValue());
            IronSource.Agent.shouldTrackNetworkState(true);
            IronSourceEvents.onBannerAdLoadedEvent += BannerLoaded;
            if (!KetchappSDK.Advertisement.HasNoAds)
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
                IronSource.Agent.loadInterstitial();
            }

            IsInitialized = true;
        }

        public void BannerLoaded()
        {
            KetchappInternal.Analytics.BannerLoaded();
        }

        public void Dispose()
        {
            IronSourceEvents.onInterstitialAdClosedEvent -= IronSource.Agent.loadInterstitial;
            IronSourceEvents.onBannerAdLoadedEvent -= BannerLoaded;
            GC.SuppressFinalize(this);
        }

        public override bool IsRewardedVideoAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        public override void ShowBanner(Action onShown)
        {
            IronSource.Agent.displayBanner();
            onShown?.Invoke();
        }

        public override bool IsInterstitialAvailable()
        {
            return IronSource.Agent.isInterstitialReady();
        }

        public override void ShowInterstitial(Action<bool> onShown)
        {
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSourceEvents.onInterstitialAdShowFailedEvent += OnInterstitialFailed;
                IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialClosed;

                IronSource.Agent.showInterstitial();

                void OnInterstitialClosed()
                {
                    KetchappInternal.Analytics.InterstitialShowed();
                    onShown?.Invoke(true);
                    IronSourceEvents.onInterstitialAdClosedEvent -= OnInterstitialClosed;
                    IronSourceEvents.onInterstitialAdShowFailedEvent -= OnInterstitialFailed;
                }

                void OnInterstitialFailed(IronSourceError ironSourceError)
                {
                    Debug.Log($"Failed to show interstitial{ironSourceError.getDescription()}");
                    onShown?.Invoke(false);
                    IronSourceEvents.onInterstitialAdClosedEvent -= OnInterstitialClosed;
                    IronSourceEvents.onInterstitialAdShowFailedEvent -= OnInterstitialFailed;
                }
            }
        }

        public override void ShowRewardedVideo(Action<bool> onRewardedFinished)
        {
            if (IsRewardedVideoAvailable())
            {
                var rvCompleted = false;
                IronSourceEvents.onRewardedVideoAdClosedEvent += OnLocalRewardedClosed;
                IronSourceEvents.onRewardedVideoAdRewardedEvent += OnLocalRewardedFinished;
                IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnLocalRewardedFailed;

                IronSource.Agent.showRewardedVideo();

                void OnLocalRewardedFinished(IronSourcePlacement iss)
                {
                    KetchappInternal.Analytics.RewardVideoValidated();
                    rvCompleted = true;
                    IronSourceEvents.onRewardedVideoAdRewardedEvent -= OnLocalRewardedFinished;
                }

                void OnLocalRewardedClosed()
                {
                    onRewardedFinished?.Invoke(rvCompleted);
                    IronSourceEvents.onRewardedVideoAdClosedEvent -= OnLocalRewardedClosed;
                }

                void OnLocalRewardedFailed(IronSourceError error)
                {
                    Debug.Log($"Ad video failed to show : {error.getDescription()}");
                    onRewardedFinished?.Invoke(rvCompleted);
                    IronSourceEvents.onRewardedVideoAdShowFailedEvent -= OnLocalRewardedFailed;
                }
            }
        }

        public override void ShowTestSuite()
        {
            IronSource.Agent.validateIntegration();
        }

        public override void RefreshUserConsent(bool result)
        {
            IronSource.Agent.hideBanner();

            IronSource.Agent.setConsent(result);

            if (!KetchappSDK.Advertisement.HasNoAds)
            {
                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
                IronSource.Agent.loadInterstitial();
                IronSource.Agent.displayBanner();
            }
        }
    }
#endif
}
