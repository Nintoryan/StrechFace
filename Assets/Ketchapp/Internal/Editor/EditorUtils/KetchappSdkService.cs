using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Ketchapp.Editor.Purchasing;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoAPI.Dto;
using Ketchapp.MayoSDK.Purchasing;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor.Utils
{
    internal class KetchappSdkService
    {
        public string KetchappSDKVersion => "1.3.0.3";
        public string KetchappSDKName => "Ketchapp Mayo SDK";

        public string GetTempPath => Path.Combine(Application.dataPath, "..");

        public List<SDKVersion> DistantSdkVersions
        {
            get
            {
                if (KetchappEditorUtils.Configuration.PlatformConfiguration == null)
                {
                    return new List<SDKVersion>();
                }

                return KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList;
            }
        }

        public List<SDKVersion> LocalSdkVersions
        {
            get
            {
                return GetLocalSDKVersions();
            }
        }

        private string GetDependenciesPath =>
            Path.Combine(Application.dataPath, "Dependencies", "Ketchapp", "Versions");

        public string GetAdapterDependenciesPath =>
            Path.Combine(Application.dataPath, "Dependencies", "Ketchapp", "Editor");

        public KetchappSdkService()
        {
            if (!Directory.Exists(GetAdapterDependenciesPath))
            {
                Directory.CreateDirectory(GetAdapterDependenciesPath);
            }

            if (!Directory.Exists(GetDependenciesPath))
            {
                Directory.CreateDirectory(GetDependenciesPath);
            }

            AssetDatabase.importPackageCompleted += OnMayoPackageImported;
        }

        public async Task DownloadAndImportSDKAsync(SDKVersion sdkVersion)
        {
            Debug.Log($"Downloading {sdkVersion.Name} SDK");

            SDKVersion distantSDK;

            if (sdkVersion.Type == SDKType.Main)
            {
                distantSDK = sdkVersion;
            }
            else
            {
                distantSDK = DistantSdkVersions.FirstOrDefault(s => s.Name == sdkVersion.Name);
            }

            if (sdkVersion == null)
            {
                return;
            }

            if (LocalSdkVersions.Any(s => s.Name == sdkVersion.Name))
            {
                var localSDk = LocalSdkVersions.FirstOrDefault(s => s.Name == sdkVersion.Name);
                if (localSDk.Version == distantSDK.Version)
                {
                    Debug.LogWarning($"SDK {sdkVersion.Name} is already up-to-date");
                    if (sdkVersion.Type == SDKType.Analytics || sdkVersion.Type == SDKType.Misc)
                    {
                        AddScriptingDefine(sdkVersion.Name);
                    }
                    else if (sdkVersion.Type == SDKType.Mediation)
                    {
                        AddScriptingDefine($"MEDIATION_{KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName}");
                    }

                    return;
                }
                else
                {
                    if (localSDk.PackageType != sdkVersion.PackageType)
                    {
                        RemoveSDK(localSDk);
                    }
                }
            }

            if (sdkVersion.PackageType == PackageType.UnityPackageRegistry)
            {
                if (sdkVersion.Name.ToLower().Contains("iap"))
                {
                    KetchappEditorUtils.UnityServicesManager.EnableUnityService(UnityServiceType.Purchasing);
                    KetchappEditorUtils.UnityServicesManager.EnableUnityService(UnityServiceType.Analytics);
                }

                KetchappEditorUtils.PackageManagerModifier.UpsertElementToManifest(sdkVersion.UnityPackageRegistryName, sdkVersion.UnityPackageRegistryVersion);
            }
            else
            {
                if (sdkVersion.Type == SDKType.Mediation)
                {
                    CheckForExistingMediation();
                }

                var path = Path.Combine(GetTempPath, $"{sdkVersion.Name}.{sdkVersion.Extension}");
                FileInfo inf;
                if (sdkVersion.Type == SDKType.Main)
                {
                    inf = await KetchappEditorUtils.MayoApiClient.DownloadFileAsync(sdkVersion.ToolVersionId, $"{sdkVersion.Name}.{sdkVersion.Extension}", CancellationToken.None);
                }
                else
                {
                    inf = await KetchappEditorUtils.MayoApiClient.DownloadFileAsync(distantSDK.ToolVersionId, $"{distantSDK.Name}.{distantSDK.Extension}", CancellationToken.None);
                }

                RemoveOldPluginFiles(sdkVersion);
                AssetDatabase.ImportPackage(inf.FullName, false);
            }

            if (sdkVersion.Type != SDKType.Main)
            {
                WritePackageVersion(distantSDK);
            }
        }

        public List<SDKVersion> GetLocalSDKVersions()
        {
            var sdkVersions = new List<SDKVersion>();
            var path = GetDependenciesPath;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var file in Directory.EnumerateFiles(path))
            {
                FileInfo info = new FileInfo(file);
                if (info.Name.Contains(".meta"))
                {
                    continue;
                }

                var json = File.ReadAllText(info.FullName);
                var sdkVersion = JsonUtility.FromJson<SDKVersion>(json);
                sdkVersions.Add(sdkVersion);
            }

            return sdkVersions;
        }

        public void WritePackageVersion(SDKVersion sdkVersion)
        {
            var path = Path.Combine(GetDependenciesPath, $"VERSION_{sdkVersion.Name.Replace(".unitypackage", string.Empty)}.json");

            var json = JsonUtility.ToJson(sdkVersion);

            File.WriteAllText(path, json);

            if (sdkVersion.Type == SDKType.Analytics || sdkVersion.Type == SDKType.Misc)
            {
                AddScriptingDefine(sdkVersion.Name);
            }
            else if (sdkVersion.Type == SDKType.Mediation)
            {
                AddScriptingDefine($"MEDIATION_{KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName}");
            }
        }

        public void RemoveSDK(SDKVersion sdk)
        {
            if (sdk.PackageType == PackageType.LegacyUnityPackage)
            {
                foreach (string path in sdk.PluginPaths)
                {
                    FileUtil.DeleteFileOrDirectory($"{Application.dataPath}/{path}");
                    FileInfo sdkFi = new FileInfo($"{Application.dataPath}/{path}");
                    sdkFi.Delete();
                }
            }
            else
            {
                if (sdk.Name.ToLower().Contains("iap"))
                {
                    KetchappEditorUtils.UnityServicesManager.DisableUnityService(UnityServiceType.Purchasing);
                }

                KetchappEditorUtils.PackageManagerModifier.RemoveElementFromManifest(sdk.UnityPackageRegistryName);
            }

            FileInfo fi = new FileInfo(Path.Combine(GetDependenciesPath, $"VERSION_{sdk.Name}.json"));
            fi.Delete();
            AssetDatabase.Refresh();

            RemoveScriptingDefine(sdk.Type == SDKType.Mediation ? $"MEDIATION_{sdk.Name}" : sdk.Name);
        }

        public void AddScriptingDefine(string define)
        {
            define = define.Replace(" ", "_");
            var targets = new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android };
            foreach (var target in targets)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                if (!symbols.Contains(define))
                {
                    symbols += $";{define}";
                    Debug.Log(symbols);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbols);
                }
            }
        }

        public void RemovePlayServiceResolver()
        {
            var path = Path.Combine(Application.dataPath, "PlayServicesResolver");
            FileUtil.DeleteFileOrDirectory(path);
            AssetDatabase.Refresh();
        }

        public void RemoveScriptingDefine(string define)
        {
            define = define.Replace(" ", "_");
            var targets = new BuildTargetGroup[] { BuildTargetGroup.iOS, BuildTargetGroup.Android };
            foreach (var target in targets)
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);
                symbols = symbols.Replace($";{define}", string.Empty);
                symbols = symbols.Replace($"{define}", string.Empty);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, symbols);
            }
        }

        public void SetUpAndroidManifestAndGradle()
        {
            var fileName = "mainTemplate.gradle";

            // For Unity 2019.3+ e can use AndroidExternalToolsSettings.gradlePath
            var mainGradlePath = Path.Combine(
                Path.GetDirectoryName(EditorApplication.applicationPath),
                "PlaybackEngines",
                "AndroidPlayer",
                "Tools",
                "GradleTemplates",
                fileName);
            var mainGradleDestinationPath = Path.Combine(Application.dataPath, "Plugins", "Android");

            if (File.Exists(Path.Combine(mainGradleDestinationPath, fileName)))
            {
                return;
            }

            if (Directory.Exists(mainGradleDestinationPath) == false)
            {
                Directory.CreateDirectory(mainGradleDestinationPath);
            }

            File.Copy(mainGradlePath, Path.Combine(mainGradleDestinationPath, fileName), false);
        }

        private void ExtractZipToDirectory(SDKZipVersion sdkVersion, ref string path)
        {
            try
            {
                var zipPath = Path.Combine(GetTempPath, $"{sdkVersion.Name}.{sdkVersion.Extension}");

                if (File.Exists(zipPath))
                {
                    Debug.Log($"Exists: {zipPath}");
                }

                path = Path.Combine(GetTempPath, sdkVersion.FolderInsideZip, $"{sdkVersion.FileName}.unitypackage");

                // Check if folder to unzip-in is already exists
                if (Directory.Exists(Path.Combine(GetTempPath, sdkVersion.FolderInsideZip)))
                {
                    Debug.Log($"Exists, deleting: {Path.Combine(GetTempPath, sdkVersion.FolderInsideZip)}");

                    Directory.Delete(Path.Combine(GetTempPath, sdkVersion.FolderInsideZip), true);
                }

                ZipFile.ExtractToDirectory(zipPath, GetTempPath);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void CheckForExistingMediation()
        {
            if (IsMediationDifferent())
            {
                var mediationSDK = LocalSdkVersions.FirstOrDefault(s => s.Type == SDKType.Mediation);
                KetchappEditorUtils.SdkService.RemoveSDK(mediationSDK);
            }
        }

        public bool IsMediationDifferent()
        {
            if (KetchappEditorUtils.Configuration.PlatformConfiguration == null)
            {
                return false;
            }

            if (LocalSdkVersions.Any(s => s.Type == SDKType.Mediation))
            {
                var mediationSDK = LocalSdkVersions.FirstOrDefault(s => s.Type == SDKType.Mediation);
                return mediationSDK.Name != KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName;
            }

            return false;
        }

        public void DownloadAdapters()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(Dependencies));
            foreach (var adapter in KetchappEditorUtils.Configuration.PlatformConfiguration.MediationAdapters)
            {
                using (var sww = new StringWriter())
                {
                    using (XmlWriter writer = XmlWriter.Create(sww))
                    {
                        Dependencies dependency = new Dependencies()
                        {
                            AndroidPackages = adapter.AndroidPackagesInfos,
                            IosPods = adapter.PodInfo,
                            Unityversion = "1.0.0"
                        };

                        formatter.Serialize(writer, dependency);
                        File.WriteAllText($"{GetAdapterDependenciesPath}/{adapter.PodInfo.FirstOrDefault()?.IosPod.Name}Dependencies.xml", sww.ToString());
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        public void RemoveAdapters()
        {
            foreach (var file in Directory.EnumerateFiles(GetAdapterDependenciesPath))
            {
                var fi = new FileInfo(file);
                if (file.Contains("Dependencies"))
                {
                    File.Delete(fi.FullName);
                }
            }

            AssetDatabase.Refresh();
        }

        public bool MediationSdkExists
        {
            get { return LocalSdkVersions.Any(s => s.Type == SDKType.Mediation); }
        }

        public bool LocalSdkHasUpdateAsync(SDKVersion sdk)
        {
            var distantSDK = DistantSdkVersions.FirstOrDefault(s => s.Name == sdk.Name);
            if (distantSDK == null)
            {
                RemoveSDK(sdk);
                return false;
            }

            var localSDK = LocalSdkVersions.FirstOrDefault(s => s.Name == sdk.Name);
            return localSDK.Version != distantSDK.Version;
        }

        public bool IsPackageInstalled(SDKVersion sdk)
        {
            if (LocalSdkVersions.Count == 0)
            {
                GetLocalSDKVersions();
            }

            return LocalSdkVersions.Any(s => s.Name == sdk.Name);
        }

        public void RemoveKetchappConfiguration()
        {
            AssetDatabase.DeleteAsset(KetchappEditorUtils.Configuration.ConfigurationObjectPath);
            AssetDatabase.Refresh();
        }

        public async Task<SDKVersion> HasNewMainSDKVersionAsync()
        {
            SDKVersionDto mainInfos = await KetchappEditorUtils.MayoApiClient.GetMayoSDKVersionAsync(CancellationToken.None);
            return mainInfos.MapFromDto();
        }

        public void DeleteOldLibraries(IEnumerable<string> filesToDelete)
        {
            if (filesToDelete == null)
            {
                Debug.Log("The list of libraries to delete is empty!");

                return;
            }

            foreach (var filePath in filesToDelete)
            {
                if (File.Exists(filePath))
                {
                    var fileAttributes = File.GetAttributes(filePath);

                    if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        Directory.Delete(filePath, true);
                    }
                    else
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }

        private void OnMayoPackageImported(string packageName)
        {
            Debug.Log("Imported ; " + packageName);
            if (packageName == KetchappSDKName)
            {
                var config = Resources.Load<IAPConfiguration>("IAPConfiguration");
                if (config != null)
                {
                    IAPEditor.CreatePurchasingEnum(config.Products);
                }
            }

            if (packageName == EditorConstants.UnityIAPPackageName)
            {
                UnityIAPImporter.ImportUnityIAP();
            }

            var packagePath = Path.Combine(Application.dataPath, "..", packageName, ".unitypackage");

            if (File.Exists(packagePath))
            {
                File.Delete(packagePath);
            }
        }

        public bool SdkCategoryHasUpdate(SDKType sDKType)
        {
            return LocalSdkVersions.Where(s => s.Type == sDKType).Any(s => LocalSdkHasUpdateAsync(s));
        }

        private void RemoveOldPluginFiles(SDKVersion sdk)
        {
            if (sdk.PluginPaths != null)
            {
                foreach (string path in sdk.PluginPaths)
                {
                    FileUtil.DeleteFileOrDirectory($"{Application.dataPath}/{path}");
                }
            }
        }
    }
}
