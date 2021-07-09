using System;
using System.Collections.Generic;
using Ketchapp.Editor.Utils;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoAPI.Dto;
using UnityEngine;

namespace Ketchapp.Internal.Configuration
{
    public static class ConfigurationMapper
    {
        public static GameConfiguration MapFromDto(this GameConfigurationDto dto)
        {
            return new GameConfiguration()
            {
                FacebookAppId = dto.FacebookAppId,
                AppsFlyerAppid = dto.AppsFlyerAppid,
                AdMobAppId = dto.AdMobAppId,
                AppsflyerApiKey = dto.AppsflyerApiKey,
                BannerId = dto.BannerId,
                CrossPromotionBundle = dto.CrossPromotionBundle,
                GameAnalyticsGameKey = dto.GameAnalyticsGameKey,
                GameAnalyticsSecretKey = dto.GameAnalyticsSecretKey,
                InterstitialId = dto.InterstitialId,
                MediationName = dto.Mediation,
                MediationAppId = dto.MediationAppId,
                RewardedVideoId = dto.RewardedVideoId,
                AndroidManifestValues = dto.AndroidManifestValues.MapConfigurationRelatedFromDtos(),
                GradleValues = dto.GradleValues.MapConfigurationRelatedFromDtos(),
                SdkList = dto.SdkList.MapFromDtos(),
                MediationAdapters = dto.MediationAdapters.MapFromDtos()
            };
        }
    }
}
