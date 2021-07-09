using System;
using Ketchapp.Internal;
using Ketchapp.MayoSDK.Advertisement;
using Ketchapp.MayoSDK.Analytics;
using Ketchapp.MayoSDK.CrossPromo;
using Ketchapp.MayoSDK.Purchasing;
using UnityEngine;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
using UnityEngine.iOS;
#endif

namespace Ketchapp.MayoSDK
{
    public class KetchappSDK : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.AddComponent(typeof(CrossPromoManager));
            gameObject.AddComponent(typeof(AdvertisementManager));
            gameObject.AddComponent(typeof(AnalyticsManager));

            CrossPromo = GetComponent<CrossPromoManager>();
            Advertisement = GetComponent<AdvertisementManager>();
            Analytics = GetComponent<AnalyticsManager>();
            Purchasing = new PurchasingManager();

            Instance = this;
            Initialize();
        }

        public static KetchappSDK Instance { get; set; }
        public static AdvertisementManager Advertisement { get; set; }
        public static AnalyticsManager Analytics { get; set; }
        public static CrossPromoManager CrossPromo { get; set; }
        public static PurchasingManager Purchasing { get; set; }
#if !CrossPromotion && UNITY_IOS
        public static ATTrackingStatusBinding.RequestAuthorizationTrackingCompleteHandler RequestAuthorizationTrackingCompleteHandler { get; private set; }
#endif
        private static void Initialize()
        {
#if (MEDIATION_MAX || MEDIATION_FairBid || MEDIATION_IronSource) && !CrossPromotion && UNITY_IOS
            RequestAuthorizationTrackingCompleteHandler += ATTHandler;

            if (Version.TryParse(Device.systemVersion, out Version deviceVersion))
            {
                if (deviceVersion >= new Version(14, 5))
                {
                    ATTrackingStatusBinding.RequestAuthorizationTracking(RequestAuthorizationTrackingCompleteHandler);
                }
            }
            else
            {
                ATTrackingStatusBinding.RequestAuthorizationTracking(RequestAuthorizationTrackingCompleteHandler);
            }
#endif

#if CrossPromotion
            CrossPromo.Initialize(() =>
            {
                Advertisement.Initialize();
                Analytics.Initialize();
            });

#else
            Advertisement.Initialize();
            Analytics.Initialize();
#endif
#if MEDIATION_MAX || MEDIATION_FairBid || MEDIATION_IronSource
            Purchasing.Initialize();
#endif

        }
#if (MEDIATION_MAX || MEDIATION_FairBid || MEDIATION_IronSource) && !CrossPromotion && UNITY_IOS
        public static void ATTHandler()
        {
            string attValue = ATTrackingStatusBinding.GetAuthorizationTrackingStatus().ToString();
            KetchappInternal.Analytics.SendAttResult(attValue);
        }
#endif
    }
}