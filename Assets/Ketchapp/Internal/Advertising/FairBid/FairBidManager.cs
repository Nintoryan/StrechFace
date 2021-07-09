using System;
using System.Collections;
using System.Collections.Generic;
using Ketchapp.Internal;
#if MEDIATION_FairBid
using Fyber;
#endif
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoSDK;
using UnityEngine;

namespace Ketchapp.Internal.Advertisement
{
#if MEDIATION_FairBid
internal class FairBidManager : AdvertisementBase, IDisposable
{
    private string AppId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.MediationAppId : KetchappInternal.ConfigurationObject.IosConfiguration.MediationAppId;
    private string InterstitialPlacementId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.InterstitialId : KetchappInternal.ConfigurationObject.IosConfiguration.InterstitialId;
    private string RewardedPlacementId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.RewardedVideoId : KetchappInternal.ConfigurationObject.IosConfiguration.RewardedVideoId;
    private string BannerPlacementId => ConfigurationType == ConfigType.Android ? KetchappInternal.ConfigurationObject.AndroidConfiguration.BannerId : KetchappInternal.ConfigurationObject.IosConfiguration.BannerId;

    public override void Initialize()
    {
        FairBid.Start(AppId);
        UserInfo.SetGdprConsent(KetchappSDK.CrossPromo.GetGdprValue());
        Interstitial.SetInterstitialListener(new FairBidInterstitialListener());
        Rewarded.SetRewardedListener(new FairBidRewardedListener());
        Banner.SetBannerListener(new FairBidBannerListener());

        Interstitial.Request(InterstitialPlacementId);
        Rewarded.Request(RewardedPlacementId);

        FairBidInterstitialListener.InterstitialHidden += OnInterstitialHidden;
        FairBidRewardedListener.RewardedHidden += OnRewardedHidden;
        IsInitialized = true;
    }

    public void Dispose()
    {
        FairBidInterstitialListener.InterstitialHidden -= OnInterstitialHidden;
        FairBidRewardedListener.RewardedHidden -= OnRewardedHidden;

        GC.SuppressFinalize(this);
    }

    public override bool IsInterstitialAvailable()
    {
        return Interstitial.IsAvailable(InterstitialPlacementId);
    }

    public override void ShowInterstitial(Action<bool> onShown)
    {
        FairBidInterstitialListener.InterstitialFailed += OnInterstitialFail;
        FairBidInterstitialListener.InterstitialHidden += OnInterstitialClosed;
        Interstitial.Show(InterstitialPlacementId);

        void OnInterstitialClosed()
        {
            onShown?.Invoke(true);
            FairBidInterstitialListener.InterstitialHidden -= OnInterstitialClosed;
            FairBidInterstitialListener.InterstitialFailed -= OnInterstitialFail;
        }

        void OnInterstitialFail()
        {
            onShown?.Invoke(false);
            FairBidInterstitialListener.InterstitialHidden -= OnInterstitialClosed;
            FairBidInterstitialListener.InterstitialFailed -= OnInterstitialFail;
        }
    }

    public override bool IsRewardedVideoAvailable()
    {
        return Rewarded.IsAvailable(RewardedPlacementId);
    }

    public override void ShowRewardedVideo(Action<bool> onRewardedFinished)
    {
        FairBidRewardedListener.RewardedShown += OnRewardedShown;
        Rewarded.Show(RewardedPlacementId);

        void OnRewardedShown(bool result)
        {
            onRewardedFinished?.Invoke(result);
            FairBidRewardedListener.RewardedShown -= OnRewardedShown;
        }
    }

    public override void ShowBanner(Action onShown)
    {
        FairBidBannerListener.BannerShown += OnBannerShown;
        Banner.Show(BannerPlacementId);

        void OnBannerShown()
        {
            onShown?.Invoke();
            FairBidBannerListener.BannerShown -= OnBannerShown;
        }
    }

    public override void HideBanner(Action onHidden = null)
    {
        Banner.Destroy(BannerPlacementId);
        onHidden?.Invoke();
    }

    public override void ShowTestSuite()
    {
        FairBid.ShowTestSuite();
    }

    private void OnRewardedHidden()
    {
        Rewarded.Request(RewardedPlacementId);
    }

    private void OnInterstitialHidden()
    {
        Interstitial.Request(InterstitialPlacementId);
    }

    public override void RefreshUserConsent(bool result)
    {
        UserInfo.SetGdprConsent(result);
        Interstitial.Request(InterstitialPlacementId);
        Rewarded.Request(RewardedPlacementId);
        Banner.Destroy(BannerPlacementId);
        Banner.Show(BannerPlacementId);
    }
}
#endif
}
