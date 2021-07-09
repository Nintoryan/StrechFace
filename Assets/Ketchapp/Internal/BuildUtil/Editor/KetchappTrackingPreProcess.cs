using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ketchapp.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class KetchappTrackingPreProcess :
#if UNITY_2018_2_OR_NEWER
IPreprocessBuildWithReport
#else
    IPreprocessBuild
#endif
{
    public int callbackOrder => 1;
#if UNITY_IPHONE
    public static string TrackFileFolder => Path.Combine(Application.dataPath, "StreamingAssets");
#elif UNITY_ANDROID
    public static string TrackFileFolder => Path.Combine(Application.dataPath, "Plugins", "Android", "res", "raw");
#else
    public static string TrackFileFolder => Path.Combine(Application.dataPath, "Dependencies", "Ketchapp", "Resources");
#endif

    public static string TrackFilePathAndroid = Path.Combine(TrackFileFolder, "configuration.json");
#if UNITY_2018_2_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
    {
        CheckFolderExistence();
        if (KetchappEditorUtils.Configuration.ConfigurationObjectExists)
        {
            var infos = GenerateConfiguration();
            var json = JsonUtility.ToJson(infos);
            File.WriteAllText(TrackFilePathAndroid, json);
            AssetDatabase.Refresh();
        }
    }
#else

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
         if (KetchappEditorUtils.Configuration.ConfigurationObjectExists)
        {
            var infos = GenerateConfiguration();
            var json = JsonUtility.ToJson(infos);
            File.WriteAllText(TrackFilePath, json);
            AssetDatabase.Refresh();
        }
    }
#endif

    private void CheckFolderExistence()
    {
        if (!Directory.Exists(TrackFileFolder))
        {
            Directory.CreateDirectory(TrackFileFolder);
        }
    }

    private GameConfigurationInformation GenerateConfiguration()
    {
        var infos = new GameConfigurationInformation();
        var config = KetchappEditorUtils.Configuration.ConfigurationObject.GetConfigurationForCurrentPlatform();

        infos.MayoVersion = KetchappEditorUtils.SdkService.KetchappSDKVersion;
        infos.UnityVersion = Application.unityVersion;

        infos.TrackedNetworkList = new List<SDKTrackingVersion>();
        infos.TrackedSdkList = new List<SDKTrackingVersion>();

        foreach (var sdk in config.SdkList)
        {
            infos.TrackedSdkList.Add(new SDKTrackingVersion()
            {
                SDKName = sdk.Name,
                SDKVersion = sdk.Version
            });
        }

        foreach (var network in config.MediationAdapters)
        {
#if UNITY_IPHONE

            foreach (var pod in network.PodInfo)
            {
                infos.TrackedNetworkList.Add(new SDKTrackingVersion()
                {
                    SDKName = pod.IosPod.Name,
                    SDKVersion = pod.IosPod.Version
                });
            }
#elif UNITY_ANDROID
            var androidPackage = network.AndroidPackagesInfos.Count == 1 ? network.AndroidPackagesInfos.First() : network.AndroidPackagesInfos.FirstOrDefault(s => s.AndroidPackage.Spec.Contains("adapter"));
            if (androidPackage != null)
            {
                var nameLength = androidPackage.AndroidPackage.Spec.LastIndexOf(":");
                infos.TrackedNetworkList.Add(new SDKTrackingVersion()
                {
                    SDKVersion = androidPackage.AndroidPackage.Spec.Substring(androidPackage.AndroidPackage.Spec.LastIndexOf(":")),
                    SDKName = androidPackage.AndroidPackage.Spec.Substring(0, nameLength)
                });
            }
#endif
        }

        return infos;
    }
}
