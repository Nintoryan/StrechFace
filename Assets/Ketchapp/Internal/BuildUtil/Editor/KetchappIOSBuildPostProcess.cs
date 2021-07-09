#if UNITY_IPHONE
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ketchapp;
using Ketchapp.Editor.Utils;
using Ketchapp.Internal.Configuration;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;
namespace Ketchapp.Internal.Build
{
    public class KetchappIOSBuildPostProcess
    {
#pragma warning disable SA1401 // Fields should be private
        public static string InfoPlistPath;
        public static string BuildPath;
        public static List<string> CrossPromoBundles;
        public static GameInfos Settings;
        public static XcodePlistStrings PlistStrings;
#pragma warning restore SA1401 // Fields should be private

        [PostProcessBuild(int.MaxValue)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuildProject)
        {
            PlistStrings = AssetDatabase.LoadAssetAtPath<XcodePlistStrings>(XcodePlistStringsEditor.AssetPath);
            Settings = Resources.Load<GameInfos>(KetchappEditorUtils.Configuration.ConfigurationObjectName);
            BuildPath = pathToBuildProject;
            if (buildTarget == BuildTarget.iOS)
            {
                string projectPath = BuildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile(projectPath);

                string target = string.Empty;
#if UNITY_2019_3_OR_NEWER
                target = pbxProject.GetUnityMainTargetGuid();
                var frameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();
#else
            target = pbxProject.ProjectGuid();
#endif
                pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                pbxProject.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");

#if MEDIATION_FairBid
                pbxProject.SetBuildProperty(target, "VALIDATE_WORKSPACE", "YES");
#endif

                ModifyInfoPlist();
#if CrossPromotion
                ModifyInterstitielBundle();
                AddCrossPromoBundle(pathToBuildProject, pbxProject, projectPath);
#endif
#if AppsFlyer
                CommentAppsflyerController();
#endif
                if (KetchappEditorUtils.SdkService.LocalSdkVersions.Any(s => s.Type == SDKType.Mediation))
                {
                    ProcessPlistLocalization(pathToBuildProject, pbxProject, target);
                }

                pbxProject.WriteToFile(projectPath);
            }
        }

        public static void ModifyInfoPlist()
        {
            InfoPlistPath = BuildPath + "/Info.plist";
            var plistParser = new PlistDocument();
            plistParser.ReadFromFile(InfoPlistPath);
            var rootDict = plistParser.root.AsDict();
            rootDict.SetString("NSCalendarsUsageDescription", "Some Ads may use this.");
            rootDict.SetString("GADApplicationIdentifier", Settings.IosConfiguration.AdMobAppId);

            foreach (var value in plistParser.root.values.ToList())
            {
                if (value.Key.Contains("UsageDescription"))
                {
                    plistParser.root.SetString(value.Key, "Some Ads may us this.");
                }
            }

            if (KetchappEditorUtils.SdkService.LocalSdkVersions.Any(s => s.Type == SDKType.Mediation))
            {
                var attLocalizedObject = PlistStrings.Properties.FirstOrDefault(c => c.LocalizationCode == LocalizationCode.EN);
                var attDesc = "Pressing \"Allow\" uses device info for more relevant ad content";
                var attProperty = "NSUserTrackingUsageDescription";
                if (attLocalizedObject != null)
                {
                    attDesc = attLocalizedObject.Properties.FirstOrDefault(c => c.Property == attProperty).Value;
                }

                rootDict.SetString(attProperty, attDesc);
            }
            string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            PlistElement tempValue;

            if (!plistParser.root.values.TryGetValue("CFBundleURLTypes", out tempValue))
            {
                plistParser.root.CreateArray("CFBundleURLTypes");
            }

            var dict = plistParser.root["CFBundleURLTypes"].AsArray().AddDict();

            dict.SetString("CFBundleURLName", bundleId);
            var bundleArray = dict.CreateArray("CFBundleURLSchemes");
            bundleArray.AddString(bundleId);
            bundleArray.AddString(Settings.IosConfiguration.CrossPromotionBundle);

            if (!plistParser.root.values.TryGetValue("LSApplicationQueriesSchemes", out tempValue))
            {
                plistParser.root.CreateArray("LSApplicationQueriesSchemes");
            }

            var bundles = plistParser.root["LSApplicationQueriesSchemes"].AsArray();
            List<string> promoBundles = RetrievePromoBundles();

            if (!BundlesAlreadyContained(bundles))
            {
                foreach (string bundle in promoBundles)
                {
                    bundles.AddString(bundle);
                }
            }

            if (KetchappEditorUtils.Configuration.ConfigurationObject.SKAdNetworksId != null)
            {
                if (KetchappEditorUtils.Configuration.ConfigurationObject.SKAdNetworksId.Count > 0)
                {
                    var skAdNetworksIdValues = KetchappEditorUtils.Configuration.ConfigurationObject.SKAdNetworksId;
                    PlistElement existingSkAdNetworks;
                    if (rootDict.values.TryGetValue("SKAdNetworkItems", out existingSkAdNetworks))
                    {
                        foreach (var existing in existingSkAdNetworks.AsArray().values)
                        {
                            PlistElement skNetworkValue;
                            if (existing.AsDict().values.TryGetValue("SKAdNetworkIdentifier", out skNetworkValue))
                            {
                                skAdNetworksIdValues.Add(skNetworkValue.AsString());
                            }
                        }
                    }

                    rootDict.values.Remove("SKAdNetworkItems");
                    plistParser.root.CreateArray("SKAdNetworkItems");

                    var skAdNetworkdsIdArray = plistParser.root["SKAdNetworkItems"].AsArray();

                    foreach (string id in skAdNetworksIdValues)
                    {
                        var skDict = skAdNetworkdsIdArray.AddDict();
                        skDict.SetString("SKAdNetworkIdentifier", id);
                    }
                }
            }

            var atsRoot = plistParser.root.CreateDict("NSAppTransportSecurity");
            atsRoot.AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
            File.WriteAllText(InfoPlistPath, plistParser.WriteToString());
        }

        public static bool BundlesAlreadyContained(PlistElementArray array)
        {
            foreach (PlistElement value in array.values.ToList())
            {
                if (value.ToString().Contains("com.ketchapp.game00"))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<string> RetrievePromoBundles()
        {
            List<string> bundles = new List<string>();

            string[] lines = File.ReadAllLines(Application.dataPath + "/Ketchapp/Internal/BuildUtil/bundles.txt");

            return lines.ToList();
        }

        public static void CommentAppsflyerController()
        {
            var path = $"{BuildPath}/Libraries/Appsflyer/Plugins/iOS/AppsFlyerAppController.mm";
            var lines = File.ReadAllText(path);
            lines = lines.Replace("IMPL_APP_CONTROLLER_SUBCLASS(AppsFlyerAppController)", string.Empty);
            File.WriteAllText(path, lines);
        }

        public static void ModifyInterstitielBundle()
        {
            string controllerPath = BuildPath + "/Libraries/Ketchapp/Plugins/iOS/Ketchapp/KetchappController.mm";
            string controllerText = File.ReadAllText(controllerPath);
            controllerText = controllerText.Replace("com.ketchapp.yourgamename", Settings.IosConfiguration.CrossPromotionBundle == string.Empty ? "com.ketchapp.game00" : Settings.IosConfiguration.CrossPromotionBundle);
            File.WriteAllText(controllerPath, controllerText);
        }

        private static void AddCrossPromoBundle(string path, PBXProject pBXProject, string projectPath)
        {
            var targetGuid = pBXProject.GetUnityMainTargetGuid();
            var pathToBundle = Path.Combine("Frameworks", "Ketchapp", "Plugins", "iOS", "Ketchapp", "Ketchapp.framework", "Resources.bundle");
            Debug.Log("GUID : " + targetGuid);
                var resourcesBuildPhase = pBXProject.GetResourcesBuildPhaseByTarget(targetGuid);
            Debug.Log("Resource build phase : " + resourcesBuildPhase);
                var resourcesFilesGuid = pBXProject.AddFile(pathToBundle, pathToBundle, PBXSourceTree.Source);
            Debug.Log("Resource build GUID : " + resourcesFilesGuid);
            pBXProject.AddFileToBuildSection(targetGuid, resourcesBuildPhase, resourcesFilesGuid);

            File.WriteAllText(projectPath, pBXProject.WriteToString());
        }

        public static void ProcessPlistLocalization(string buildPath, PBXProject pBXProject, string targetGuid)
        {
            foreach (var property in PlistStrings.Properties)
            {
                var locale = property.LocalizationCode;
                var localeString = property.LocalizationCode.ToString().ToLower();
                if (locale == LocalizationCode.CN)
                {
                    localeString = "zh-Hans";
                }

                foreach (var entry in property.Properties)
                {
                    if (!string.IsNullOrEmpty(entry.Property) && !string.IsNullOrEmpty(entry.Value))
                    {
                        LocalizePlistEntries(entry, localeString, buildPath, pBXProject, targetGuid);
                    }
                }
            }
        }

        private static void LocalizePlistEntries(PlistProperty entryDescription, string localeCode, string buildPath, PBXProject project, string targetGuid)
        {
            const string resourcesDirectoryName = "LocalizedResources";
            var localeSpecificDirectoryName = localeCode + ".lproj";
            var localeSpecificDirectoryPath = Path.Combine(resourcesDirectoryName, localeSpecificDirectoryName);
            var infoPlistStringsFilePath = Path.Combine(buildPath, localeSpecificDirectoryPath, "InfoPlist.strings");

            // Create intermediate directories as needed.
            if (!Directory.Exists(Path.Combine(buildPath, resourcesDirectoryName)))
            {
                Directory.CreateDirectory(Path.Combine(buildPath, resourcesDirectoryName));
            }

            if (!Directory.Exists(Path.Combine(buildPath, localeSpecificDirectoryPath)))
            {
                Directory.CreateDirectory(Path.Combine(buildPath, localeSpecificDirectoryPath));
            }

            var localizedDescriptionLine = $"{entryDescription.Property} = \"{entryDescription.Value} \";\n";

            if (File.Exists(infoPlistStringsFilePath))
            {
                var output = new List<string>();
                var lines = File.ReadAllLines(infoPlistStringsFilePath);
                var keyUpdated = false;
                foreach (var line in lines)
                {
                    if (line.Contains(entryDescription.Property))
                    {
                        output.Add(localizedDescriptionLine);
                        keyUpdated = true;
                    }
                    else
                    {
                        output.Add(line);
                    }
                }

                if (!keyUpdated)
                {
                    output.Add(localizedDescriptionLine);
                }

                File.WriteAllText(infoPlistStringsFilePath, string.Join("\n", output.ToArray()) + "\n");
            }
            else
            {
                File.WriteAllText(infoPlistStringsFilePath, "/* Localized versions of Info.plist keys*/\n" + localizedDescriptionLine);
            }

            var guid = project.AddFolderReference(localeSpecificDirectoryPath, localeSpecificDirectoryPath);
            project.AddFileToBuild(targetGuid, guid);
        }
    }
}
#endif