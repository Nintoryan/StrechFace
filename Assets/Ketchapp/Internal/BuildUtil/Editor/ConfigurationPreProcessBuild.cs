using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ketchapp.Editor;
using Ketchapp.Editor.Utils;
#if FacebookSDK
using Facebook.Unity.Editor;
#endif
#if GameAnalytics
using GameAnalyticsSDK;
#endif
using Ketchapp.Internal.Configuration;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_2_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;

namespace Ketchapp.Internal.BuildUtil
{
    public class ConfigurationPreProcessBuild :
#if UNITY_2018_2_OR_NEWER
IPreprocessBuildWithReport
#else
    IPreprocessBuild
#endif
    {
        public int callbackOrder => 0;

#if UNITY_2018_2_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            var configuration = KetchappEditorUtils.Configuration.ConfigurationObject;
            if (KetchappEditorUtils.Configuration.PlatformConfiguration == null)
            {
                throw new System.Exception("[Mayo SDK] : Active target build has no configuration, fetch configuration in Ketchapp Mayo SDK > Setup and/or switch to correct target in Build Settings.");
            }

            KetchappCustomConstantsBuilder.Build();
#if GameAnalytics
            var gASettings = Resources.Load<GameAnalyticsSDK.Setup.Settings>("GameAnalytics/Settings");
            if (!gASettings)
            {
                Selection.activeObject = GameAnalytics.SettingsGA;
            }

            RemoveGAEditorFields(gASettings);
            gASettings.AddPlatform(RuntimePlatform.Android);
            gASettings.UpdateGameKey(0, configuration.AndroidConfiguration.GameAnalyticsGameKey);
            gASettings.UpdateSecretKey(0, configuration.AndroidConfiguration.GameAnalyticsSecretKey);

            gASettings.AddPlatform(RuntimePlatform.IPhonePlayer);
            gASettings.UpdateSecretKey(1, configuration.IosConfiguration.GameAnalyticsSecretKey);
            gASettings.UpdateGameKey(1, configuration.IosConfiguration.GameAnalyticsGameKey);
            gASettings.UsePlayerSettingsBuildNumber = true;
            EditorUtility.SetDirty(gASettings);
            AssetDatabase.SaveAssets();
#endif

#if FacebookSDK
            var fbSettings = Resources.Load<Facebook.Unity.Settings.FacebookSettings>("FacebookSettings");
            if (!fbSettings)
            {
                var asset = ScriptableObject.CreateInstance(typeof(Facebook.Unity.Settings.FacebookSettings));
                AssetDatabase.CreateAsset(asset, $"Assets/Resources/FacebookSettings.asset");
                AssetDatabase.Refresh();
                fbSettings = asset as Facebook.Unity.Settings.FacebookSettings;
            }

            Facebook.Unity.Settings.FacebookSettings.AppIds.Clear();
            Facebook.Unity.Settings.FacebookSettings.AppIds.Add(configuration.IosConfiguration.FacebookAppId);
            EditorUtility.SetDirty(fbSettings);
            ManifestMod.GenerateManifest();
#endif
#if MEDIATION_MAX
#if UNITY_IOS || UNITY_IPHONE
            AppLovinSettings.Instance.SdkKey = KetchappEditorUtils.Configuration.ConfigurationObject.IosConfiguration.MediationAppId;
#elif UNITY_ANDROID
            AppLovinSettings.Instance.SdkKey = KetchappEditorUtils.Configuration.ConfigurationObject.AndroidConfiguration.MediationAppId;
#endif
            EditorUtility.SetDirty(AppLovinSettings.Instance);
#endif
        }
#else
            public void OnPreprocessBuild(BuildTarget target, string path)
    {
        var configuration = KetchappEditorUtils.Configuration.ConfigurationObject;

#if GameAnalytics
        var gASettings = Resources.Load<GameAnalyticsSDK.Setup.Settings>("GameAnalytics/Settings");
        if (!gASettings)
        {
            Selection.activeObject = GameAnalytics.SettingsGA;
        }

            RemoveGAEditorFields(gASettings);
            gASettings.AddPlatform(RuntimePlatform.Android);
            gASettings.UpdateGameKey(0, configuration.AndroidConfiguration.GameAnalyticsGameKey);
            gASettings.UpdateSecretKey(0, configuration.AndroidConfiguration.GameAnalyticsSecretKey);

            gASettings.AddPlatform(RuntimePlatform.IPhonePlayer);
            gASettings.UpdateSecretKey(1, configuration.IosConfiguration.GameAnalyticsSecretKey);
            gASettings.UpdateGameKey(1, configuration.IosConfiguration.GameAnalyticsGameKey);
            gASettings.UsePlayerSettingsBuildNumber = true;
            EditorUtility.SetDirty(gASettings);
            AssetDatabase.SaveAssets();
#endif

#if FacebookSDK
        var fbSettings = Resources.Load<Facebook.Unity.Settings.FacebookSettings>("FacebookSettings");
        Facebook.Unity.Settings.FacebookSettings.AppIds.Clear();
        Facebook.Unity.Settings.FacebookSettings.AppIds.Add(configuration.IosConfiguration.FacebookAppId);
        EditorUtility.SetDirty(fbSettings);
#endif
    }
#endif
#if GameAnalytics
        private void RemoveGAEditorFields(GameAnalyticsSDK.Setup.Settings gASettings)
        {
            gASettings.Platforms.Clear();
            gASettings.Build.Clear();
            gASettings.SelectedPlatformGame.Clear();
            gASettings.SelectedPlatformStudio.Clear();
            gASettings.PlatformFoldOut.Clear();
            EditorUtility.SetDirty(gASettings);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
