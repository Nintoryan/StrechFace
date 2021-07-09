#if UNITY_ANDROID
using System.IO;
using Ketchapp.Editor.Utils;
using Ketchapp.Internal.BuildUtil.Gradle;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoAPI.Dto;
using UnityEditor;
using UnityEditor.Build;
#if UNITY_2018_2_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
namespace Ketchapp.Internal.BuildUtil
{
public class KetchappAndroidBuildPostProcess :
#if UNITY_2018_2_OR_NEWER
IPreprocessBuildWithReport
#else
    IPreprocessBuild
#endif
{
    public int callbackOrder => 0;

    private string MainGradlePath => Path.Combine(Application.dataPath, "Plugins", "Android", "mainTemplate.gradle");

#if UNITY_2018_2_OR_NEWER
    public void OnPreprocessBuild(BuildReport report)
    {
        SetUpAndroid();
        var configurationData = KetchappEditorUtils.Configuration.ConfigurationObject.AndroidConfiguration.GradleValues;

        if (configurationData.Count == 0)
        {
            Debug.Log("Config is null");
            return;
        }

        var mainGradleFile = new GradleModifier(MainGradlePath);

        foreach (var data in configurationData)
        {
            var node = mainGradleFile.Root.TryGetNode(data.Name);

            node.AppendContentNode(data.Value);
        }

        mainGradleFile.Save();
    }
#else
public void OnPreprocessBuild(BuildTarget target, string path)
    {
        SetUpAndroid();
        var configurationData = KetchappEditorUtils.Configuration.ConfigurationObject.AndroidConfiguration.GradleValues;

        if (configurationData.Count == 0)
        {
            Debug.Log("Config is null");
            return;
        }

        var mainGradleFile = new GradleModifier(MainGradlePath);

        foreach (var data in configurationData)
        {
            var node = mainGradleFile.Root.TryGetNode(data.Name);

            node.AppendContentNode(data.Value);
        }

        mainGradleFile.Save();
    }
#endif

    private void SetUpAndroid()
    {
#if External_Dependency_Manager
#endif

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel21;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)29;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;

        if ((GameStateDto)KetchappEditorUtils.Configuration.ConfigurationObject.GameState == GameStateDto.Live)
        {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = Path.Combine(Application.dataPath, "Ketchapp", "Internal", "BuildUtil", "keystore_ketchapp.keystore");
                PlayerSettings.Android.keystorePass = "orcom963";
                PlayerSettings.Android.keyaliasName = "keystore";
                PlayerSettings.Android.keyaliasPass = "orcom963";
        }
    }
}
}
#endif
