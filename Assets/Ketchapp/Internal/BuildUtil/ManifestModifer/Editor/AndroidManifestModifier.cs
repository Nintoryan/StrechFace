using Ketchapp.Editor.Utils;
using Ketchapp.Internal.Configuration;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEngine;

namespace Ketchapp.Internal.BuildUtil.Editor.ManifestModifer
{
    internal class AndroidManifestModifier : 
#if UNITY_2018_1_OR_NEWER
        IPostGenerateGradleAndroidProject
#else
        IPreprocessBuild
#endif
    {
        public int callbackOrder => 1;

#if UNITY_2018_1_OR_NEWER
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var configuration = Resources.Load<GameInfos>(KetchappEditorUtils.Configuration.ConfigurationObjectName).AndroidConfiguration;
            var className = GetType().UnderlyingSystemType.Name;

            Debug.Log($"[KetchappAndroidManifestModifier] Modifying your AndroidManifest. {className}");
            var manifestPath = ManifestUtils.GetManifestPath(path);
            var androidManifest = new AndroidManifest(manifestPath);
            Modify(androidManifest, configuration);
            androidManifest.Save();
        }
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            var configuration = Resources.Load<GameInfos>(KetchappEditorUtils.Configuration.ConfigurationObjectName).AndroidConfiguration;
            var className = GetType().UnderlyingSystemType.Name;

            Debug.Log($"[KetchappAndroidManifestModifier] Modifying your AndroidManifest. {className}");
            var manifestPath = ManifestUtils.GetManifestPath(path);
            var androidManifest = new AndroidManifest(manifestPath);
            Modify(androidManifest, configuration);
            androidManifest.Save();
        }
#endif  

        private void Modify(AndroidManifest androidManifest, GameConfiguration configuration)
        {
            foreach (var config in configuration.AndroidManifestValues)
            {
                androidManifest.SetAttributeInManifest((AndroidManifestType)config.ManifestType, config.Name, config.Value);
            }
        }
    }
}