using System;
using Ketchapp.Internal;
using Ketchapp.Internal.Configuration;
using UnityEngine;

namespace Ketchapp.Internal.Advertisement
{
    internal class AdvertisementManager : AdvertisementBase
    {
        private readonly AdvertisementBase _advertisement;

        private DateTime _lastInterstitialShowed;

        public AdvertisementManager()
        {
#if UNITY_EDITOR
            _advertisement = new DebugAdvertisementManager();
#elif MEDIATION_IronSource
            _advertisement = new IronSourceManager();
#elif MEDIATION_MAX
    _advertisement = new MAXManager();
#elif MEDIATION_FairBid
    _advertisement = new FairBidManager();
#endif
        }

        public override void HideBanner(Action onHidden = null)
        {
            _advertisement.HideBanner();
        }

        public override void Initialize()
        {
            if (_advertisement != null)
            {
                _advertisement.Initialize();
            }
            else
            {
                Debug.Log("Advertisement configuration not present");
            }
        }

        public override bool IsInterstitialAvailable()
        {
            if (_advertisement != null)
            {
                return _advertisement.IsInterstitialAvailable();
            }

            Debug.Log("Advertisement configuration not present");
            return false;
        }

        public override bool IsRewardedVideoAvailable()
        {
            if (_advertisement != null)
            {
                return _advertisement.IsRewardedVideoAvailable();
            }

            Debug.Log("Advertisement configuration not present");
            return false;
        }

        public override void ShowBanner(Action onShown)
        {
            if (_advertisement != null)
            {
                if (MayoSDK.KetchappSDK.Advertisement.HasNoAds)
                {
                    onShown?.Invoke();
                    return;
                }

                _advertisement.ShowBanner(() =>
                {
                    KetchappInternal.Analytics.BannerLoaded();
                    onShown?.Invoke();
                });
            }
            else
            {
                onShown?.Invoke();
                Debug.Log("Advertisement configuration not present");
            }
        }

        public override void ShowInterstitial(Action<bool> onShown)
        {
            if (_advertisement != null)
            {
                if (_lastInterstitialShowed != null)
                {
                    if ((DateTime.Now - _lastInterstitialShowed).TotalSeconds < KetchappInternal.ConfigurationConstants.InterstitialDelay)
                    {
                        onShown?.Invoke(false);
                        return;
                    }
                }

                if (MayoSDK.KetchappSDK.Advertisement.HasNoAds)
                {
                    onShown?.Invoke(false);
                    return;
                }

                _advertisement.ShowInterstitial((bool res) =>
                {
                    onShown?.Invoke(res);
                    _lastInterstitialShowed = DateTime.Now;
                });
            }
            else
            {
                onShown?.Invoke(false);
                Debug.Log("Advertisement configuration not present");
            }
        }

        public override void ShowRewardedVideo(Action<bool> onRewardedFinished)
        {
            if (_advertisement != null)
            {
                _advertisement.ShowRewardedVideo((isRewarded) =>
                {
                    onRewardedFinished?.Invoke(isRewarded);
                    _lastInterstitialShowed = DateTime.Now;
                });
            }
            else
            {
                onRewardedFinished?.Invoke(false);
                Debug.Log("Advertisement configuration no present");
            }
        }

        public override void ShowTestSuite()
        {
            if (_advertisement != null)
            {
                _advertisement.ShowTestSuite();
            }
            else
            {
                Debug.Log("Advertisement configuration no present");
            }
        }

        public override void RefreshUserConsent(bool result)
        {
            _advertisement?.RefreshUserConsent(result);
        }

        public bool IsSdkInitialized()
        {
            return _advertisement.IsInitialized;
        }
    }
}
