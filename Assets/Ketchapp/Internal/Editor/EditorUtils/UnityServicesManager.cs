using System;
using System.Collections.Generic;
using System.IO;
using Ketchapp.Editor.Utils;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;

public class UnityServicesManager
{
    private string UnityConnectPath => Path.Combine(Application.dataPath, "..", "ProjectSettings", "UnityConnectSettings.asset");

    private (ConnectSettings Object, string prefix) ReadConnectAsset()
    {
        var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
        var lines = File.ReadAllText(UnityConnectPath);
        var suffix = lines.Substring(lines.IndexOf("UnityConnectSettings"));
        var prefix = lines.Replace(suffix, string.Empty);
        return (deserializer.Deserialize<ConnectSettings>(suffix), prefix);
    }

    public void EnableUnityService(UnityServiceType service)
    {
        var serializer = new Serializer();
        try
        {
            var file = ReadConnectAsset();

            KetchappEditorUtils.SdkService.AddScriptingDefine("UNITY_PURCHASING");

            switch (service)
            {
                case UnityServiceType.Analytics:
                    {
                        file.Object.UnityConnectSettings.UnityAnalyticsSettings.MEnabled = 1;
                        break;
                    }

                case UnityServiceType.Purchasing:
                    {
                        file.Object.UnityConnectSettings.UnityPurchasingSettings.MEnabled = 1;
                        break;
                    }

                default:
                    break;
            }

            var yaml = serializer.Serialize(file.Object);
            yaml = yaml.Insert(0, file.prefix);
            File.WriteAllText(UnityConnectPath, yaml);
            AssetDatabase.Refresh();
        }
        catch
        {
            Debug.LogError("[Mayo SDK]: Please set Asset Serialization Mode to \"FORCE TEXT\" in Project Settings > Editor or manually enable Unity IAP and Analytics services");
            EditorUtility.DisplayDialog("YAML serialization error", "Please set to \"FORCE TEXT\" in Project Settings > Editor > Asset Serialization > Mode ", "Ok");
        }
    }

    public void DisableUnityService(UnityServiceType service)
    {
        var serializer = new Serializer();
        var file = ReadConnectAsset();

        KetchappEditorUtils.SdkService.RemoveScriptingDefine("UNITY_PURCHASING");

        switch (service)
        {
            case UnityServiceType.Analytics:
                {
                    file.Object.UnityConnectSettings.UnityAnalyticsSettings.MEnabled = 0;
                    break;
                }

            case UnityServiceType.Purchasing:
                {
                    file.Object.UnityConnectSettings.UnityPurchasingSettings.MEnabled = 0;
                    break;
                }

            default:
                break;
        }

        var yaml = serializer.Serialize(file.Object);
        yaml = yaml.Insert(0, file.prefix);
        File.WriteAllText(UnityConnectPath, yaml);
    }
}

public enum UnityServiceType
{
    Purchasing,
    Analytics
}

public partial class ConnectSettings
{
    [YamlMember(Alias = "UnityConnectSettings")]
    public UnityConnectSettings UnityConnectSettings { get; set; }
}

public partial class UnityConnectSettings
{
    [YamlMember(Alias = "m_ObjectHideFlags")]
    public long MObjectHideFlags { get; set; }

    [YamlMember(Alias = "serializedVersion")]
    public long SerializedVersion { get; set; }

    [YamlMember(Alias = "m_Enabled")]
    public long MEnabled { get; set; }

    [YamlMember(Alias = "m_TestMode")]
    public long MTestMode { get; set; }

    [YamlMember(Alias = "m_EventOldUrl")]
    public string MEventOldUrl { get; set; }

    [YamlMember(Alias = "m_EventUrl")]
    public string MEventUrl { get; set; }

    [YamlMember(Alias = "m_ConfigUrl")]
    public string MConfigUrl { get; set; }

    [YamlMember(Alias = "m_TestInitMode")]
    public long MTestInitMode { get; set; }

    [YamlMember(Alias = "CrashReportingSettings")]
    public CrashReportingSettings CrashReportingSettings { get; set; }

    [YamlMember(Alias = "UnityPurchasingSettings")]
    public UnityPurchasingSettings UnityPurchasingSettings { get; set; }

    [YamlMember(Alias = "UnityAnalyticsSettings")]
    public UnityAnalyticsSettings UnityAnalyticsSettings { get; set; }

    [YamlMember(Alias = "UnityAdsSettings")]
    public UnityAdsSettings UnityAdsSettings { get; set; }

    [YamlMember(Alias = "PerformanceReportingSettings")]
    public PerformanceReportingSettings PerformanceReportingSettings { get; set; }
}

public partial class CrashReportingSettings
{
    [YamlMember(Alias = "m_EventUrl")]
    public string MEventUrl { get; set; }

    [YamlMember(Alias = "m_Enabled")]
    public long MEnabled { get; set; }

    [YamlMember(Alias = "m_LogBufferSize")]
    public long MLogBufferSize { get; set; }

    [YamlMember(Alias = "m_CaptureEditorExceptions")]
    public long MCaptureEditorExceptions { get; set; }
}

public partial class PerformanceReportingSettings
{
    [YamlMember(Alias = "m_Enabled")]
    public long MEnabled { get; set; }
}

public partial class UnityAdsSettings
{
    [YamlMember(Alias = "m_Enabled")]
    public long MEnabled { get; set; }

    [YamlMember(Alias = "m_InitializeOnStartup")]
    public long MInitializeOnStartup { get; set; }

    [YamlMember(Alias = "m_TestMode")]
    public long MTestMode { get; set; }

    [YamlMember(Alias = "m_IosGameId")]
    public object MIosGameId { get; set; }

    [YamlMember(Alias = "m_AndroidGameId")]
    public object MAndroidGameId { get; set; }

    [YamlMember(Alias = "m_GameIds")]
    public MGameIds MGameIds { get; set; }

    [YamlMember(Alias = "m_GameId")]
    public object MGameId { get; set; }
}

public partial class MGameIds
{
}

public partial class UnityAnalyticsSettings
{
    [YamlMember(Alias = "m_Enabled")]
    public long MEnabled { get; set; }

    [YamlMember(Alias = "m_TestMode")]
    public long MTestMode { get; set; }

    [YamlMember(Alias = "m_InitializeOnStartup")]
    public long MInitializeOnStartup { get; set; }
}

public partial class UnityPurchasingSettings
{
    [YamlMember(Alias = "m_Enabled")]
    public long MEnabled { get; set; }

    [YamlMember(Alias = "m_TestMode")]
    public long MTestMode { get; set; }
}
