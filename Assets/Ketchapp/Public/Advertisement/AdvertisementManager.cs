using System;
using System.Collections;
using Ketchapp.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Ketchapp.MayoSDK.Advertisement
{
    public class AdvertisementManager : MonoBehaviour
    {
        public bool BannerDisplayed
        {
            get
            {
                return _bannerDisplayed;
            }
            set
            {
                if (value != _bannerDisplayed)
                {
                    _bannerDisplayed = value;
                    BannerBackground.SetActive(value);
                }
            }
        }

        private bool _bannerDisplayed;

        public bool HasNoAds
        {
            get
            {
                return PlayerPrefs.GetInt(KetchappInternal.PurchasingManager.NoAdsKey, 0) == 1;
            }
        }

        private GameObject BannerBackground { get; set; }

        private void Awake()
        {
            var bannerObject = Resources.Load<GameObject>("BannerBackground");
            BannerBackground = Instantiate(bannerObject);
            DontDestroyOnLoad(BannerBackground);
            var bannerImage = BannerBackground.GetComponentInChildren<Image>();
            bannerImage.rectTransform.sizeDelta = new Vector2(bannerImage.rectTransform.sizeDelta.x, KetchappInternal.ConfigurationConstants.BannerHeight);
            BannerBackground.SetActive(false);
        }

        /// <summary>
        /// Tell the mediation to hide the banner.
        /// </summary>
        public void HideBanner()
        {
            KetchappInternal.Advertisement.HideBanner();
            BannerDisplayed = false;
        }

        public void Initialize()
        {
            KetchappInternal.Advertisement.Initialize();
        }

        /// <summary>
        /// Returns whether or not the rewarded video is available to show.
        /// </summary>
        /// <returns></returns>
        public bool IsRewardedVideoAvailable()
        {
            return KetchappInternal.Advertisement.IsRewardedVideoAvailable();
        }

        /// <summary>
        /// Tell the mediation to show the banner. It won't show if the user purchased NoAds (if any).
        /// </summary>
        /// <param name="onDone">Optional callback when banner is shown.</param>
        public void ShowBanner(Action onDone = null)
        {
            StartCoroutine(WaitUntilInitialized(() =>
            {
                KetchappInternal.Advertisement.ShowBanner(() =>
                {
                    onDone?.Invoke();
                    BannerDisplayed = !KetchappSDK.Advertisement.HasNoAds;
                });
            }));
        }

        /// <summary>
        /// Show banner with custom background color. It won't show if the user purchased NoAds (if any).
        /// </summary>
        /// <param name="bannerColor">Color to set. Don't forget to set alpha.</param>
        /// <param name="onDone">Optional callback when banner is shown.</param>
        public void ShowBanner(Color bannerColor, Action onDone = null)
        {
            StartCoroutine(WaitUntilInitialized(() =>
            {
                KetchappInternal.Advertisement.ShowBanner(() =>
                {
                    onDone?.Invoke();
                    BannerDisplayed = !KetchappSDK.Advertisement.HasNoAds;
                });
            }));
        }

        /// <summary>
        /// Tell the mediation to show an interstitial. It won't show if user purchased NoAds (if any).
        /// </summary>
        /// <param name="onDone">Optional callback when interstitial is closed.</param>
        public void ShowInterstitial(Action<bool> onDone = null)
        {
            KetchappInternal.Advertisement.ShowInterstitial(onDone);
        }

        /// <summary>
        /// Tell the mediation to show a rewarded video.
        /// </summary>
        /// <param name="rewardedFinished">Mandatory callback when RV is closed and the user should be rewarded or not.</param>
        public void ShowRewardedVideo(Action<bool> rewardedFinished)
        {
            KetchappInternal.Advertisement.ShowRewardedVideo(rewardedFinished);
        }

        /// <summary>
        /// For debugging purpose, will show the mediation test suite (if any).
        /// </summary>
        public void ShowTestSuite()
        {
            KetchappInternal.Advertisement.ShowTestSuite();
        }

        /// <summary>
        /// Returns whether or not an interstitial is available to display.
        /// </summary>
        /// <returns></returns>
        public bool IsInterstitialAvailable()
        {
            return KetchappInternal.Advertisement.IsInterstitialAvailable();
        }

        private IEnumerator WaitUntilInitialized(Action onDone)
        {
            yield return new WaitUntil(() => KetchappInternal.Advertisement.IsSdkInitialized());
            onDone?.Invoke();
        }
    }
}
