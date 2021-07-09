using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ketchapp.Internal.Configuration
{
    [Serializable]
    public class GameConfiguration
    {
#pragma warning disable SA1401 // Fields should be private
        [SerializeField] public string FacebookAppId;
        [SerializeField] public string GameAnalyticsSecretKey;
        [SerializeField] public string GameAnalyticsGameKey;
        [SerializeField] public string MediationAppId;
        [SerializeField] public string MediationName;
        [SerializeField] public string AdMobAppId;
        [SerializeField] public string RewardedVideoId;
        [SerializeField] public string InterstitialId;
        [SerializeField] public string BannerId;
        [SerializeField] public string AppsflyerApiKey;
        [SerializeField] public string AppsFlyerAppid;
        [SerializeField] public string CrossPromotionBundle;
        [SerializeField] public List<ConfigurationRelated> AndroidManifestValues;
        [SerializeField] public List<ConfigurationRelated> GradleValues;
        [SerializeField] public List<SDKVersion> SdkList;
        [SerializeField] public List<MediationAdapter> MediationAdapters;

#pragma warning restore SA1401 // Fields should be private

        [Serializable]
        public class ConfigurationRelated
        {
#pragma warning disable SA1401 // Fields should be private
            public int ManifestType;
            public string Name;
            public string Value;
#pragma warning restore SA1401 // Fields should be private
        }
    }
}