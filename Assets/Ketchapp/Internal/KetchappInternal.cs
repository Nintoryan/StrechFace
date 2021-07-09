using System;
using Ketchapp.Internal.Advertisement;
using Ketchapp.Internal.Analytics;
using Ketchapp.Internal.Configuration;
using Ketchapp.Internal.CrossPromo;
using Ketchapp.Internal.Purchasing;
using Ketchapp.MayoSDK;
using Ketchapp.MayoSDK.Purchasing;
using UnityEngine;

namespace Ketchapp.Internal
{
    internal static class KetchappInternal
    {
        static KetchappInternal()
        {
            Analytics = new AnalyticsManager();
            Advertisement = new AdvertisementManager();
            ConfigurationObject = Resources.Load<GameInfos>("KetchappSettings");
            IAPConfiguration = Resources.Load<IAPConfiguration>("IAPConfiguration");
#if UNITY_EDITOR
            ConfigurationConstants = KetchappCustomConstantsBuilder.Build();
#else
            var constants = Resources.Load<KetchappCustomConstants>("KetchappConstants");
            if (constants == null)
            {
                ConfigurationConstants = ScriptableObject.CreateInstance<KetchappCustomConstants>();
            }
            else
            {
                ConfigurationConstants = constants;
            }
#endif

            PurchasingManager = new Purchasing.IAPManager();
        }

        public static IAPConfiguration IAPConfiguration { get; set; }
        public static GameInfos ConfigurationObject { get; set; }
        public static AdvertisementManager Advertisement { get; set; }
        public static AnalyticsManager Analytics { get; set; }
        public static CrossPromoManager CrossPromo { get; set; }
        public static KetchappCustomConstants ConfigurationConstants { get; set; }
        public static Purchasing.IAPManager PurchasingManager { get; set; }
    }
}