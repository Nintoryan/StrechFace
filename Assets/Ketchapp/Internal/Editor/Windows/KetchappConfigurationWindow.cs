using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Assets.Ketchapp.Internal.Editor.EditorUtils.Dto;
using Ketchapp.Editor.Utils;
using Ketchapp.Internal.Configuration;
using Ketchapp.MayoAPI.Dto;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor
{
    internal static class KetchappConfigurationWindow
    {
        private static int _selectedStudioId;
        private static int _selectedGameId = 0;
        private static bool isDownloadingSdks;
        private static int sdkDownloaded;
        private static int sdkCount;
        private static string downloadingSdkName;
        private static Color buttonColor;

        private static bool isFetchingConfig;
        public static bool ConfigInitialized;
        private static bool _initializinConfig;

        private static GUIStyle _iconStyle = new GUIStyle();

        static KetchappConfigurationWindow()
        {
        }

        public static async Task InitializeConfig()
        {
            if (!ConfigInitialized)
            {
                _initializinConfig = true;
                ConfigInitialized = true;

                await KetchappEditorUtils.Configuration.PopulateStudioListAsync();
               
                if (KetchappEditorUtils.Configuration.ConfigurationObjectExists)
                {
                    await RetrieveConfigurationObject();
                }

                _initializinConfig = false;
            }
        }

        public static async void ShowConfigurationPanel()
        {
            buttonColor = GUI.backgroundColor;

            if (KetchappEditorUtils.AuthenticationManager.IsUserAuthenticated)
            {
                if (GUI.changed)
                {
                    var existingConfig = GetExistingConfiguration();
                    _selectedGameId = existingConfig.GameId;
                    _selectedStudioId = existingConfig.StudioId;
                }

                if (!ConfigInitialized)
                {
                    await InitializeConfig();
                    return;
                }

                if (_initializinConfig)
                {
                    KetchappEditorHelper.LoadingText();
                    return;
                }

                if (KetchappEditorUtils.Configuration.ConfigurationObjectExists)
                {
                    var guiColor = GUI.color;

                    CheckUnityVersionsCompatibility(out var versionNumber);

                    if (!string.IsNullOrEmpty(versionNumber))
                    {
                        GUI.color = Color.red;
                        EditorGUILayout.HelpBox($"This Unity version has a bug. Please update to at least {versionNumber}", MessageType.Info);
                    }

                    switch ((GameStateDto)KetchappEditorUtils.Configuration.ConfigurationObject.GameState)
                    {
                        case GameStateDto.Killed:
                            {
                                GUI.color = Color.red;
                                EditorGUILayout.HelpBox("Your game is currently in Killed State", MessageType.Info);
                                break;
                            }

                        case GameStateDto.Live:
                            {
                                GUI.color = Color.green;
                                EditorGUILayout.HelpBox("Your game is currently in Live State", MessageType.Info);
                                break;
                            }

                        case GameStateDto.SL:
                            {
                                if (KetchappEditorUtils.SdkService.DistantSdkVersions.Any(s => s.Type == SDKType.Mediation))
                                {
                                    GUI.color = Color.cyan;
                                    EditorGUILayout.HelpBox("Your game is currently in Softlaunch State", MessageType.Info);
                                }
                                else
                                {
                                    GUI.color = Color.yellow;
                                    EditorGUILayout.HelpBox("Your game is currently in Softlaunch State", MessageType.Info);
                                }

                                break;
                            }

                        default:
                            break;
                    }

                    GUI.color = guiColor;
                }

                KetchappEditorHelper.Label("Configuration", 15);
                KetchappEditorHelper.GuiLine(GUI.skin);
                EditorGUILayout.Space();

                if (KetchappEditorUtils.Configuration.UserStudios.Count() <= 0)
                {
                    EditorGUILayout.HelpBox("You have no studios/games available", MessageType.Error);
                    return;
                }

                _selectedStudioId = EditorGUILayout.Popup("Select your studio", _selectedStudioId, KetchappEditorUtils.Configuration.UserStudios);
                _selectedGameId = EditorGUILayout.Popup("Select your Game", _selectedGameId, KetchappEditorUtils.Configuration.GetGamesNameById(_selectedStudioId));

                KetchappEditorHelper.GuiLine(GUI.skin);
                EditorGUILayout.Space();

                GUI.enabled = !isFetchingConfig;
                if (GUILayout.Button(KetchappEditorUtils.Configuration.ConfigurationObjectExists ? "Update configuration object" : "Create configuration object"))
                {
                    isFetchingConfig = true;
                    await RetrieveConfigurationObject();
                    isFetchingConfig = false;

                    if (KetchappEditorUtils.Configuration.ConfigurationObjectExists)
                    {
                        KetchappEditorUtils.SdkService.RemoveAdapters();
                        KetchappEditorUtils.SdkService.DownloadAdapters();
                    }

                    return;
                }

                GUI.enabled = true;

                if (KetchappEditorUtils.Configuration.PlatformConfiguration != null)
                {
                    KetchappEditorHelper.GuiLine(GUI.skin);
                    KetchappEditorHelper.Label("Download SDKs", 15);
                    GUI.enabled = !isDownloadingSdks && KetchappEditorUtils.Configuration.ConfigurationObjectExists;
                    if (!KetchappEditorUtils.SdkService.IsMediationDifferent())
                    {
                        EditorGUILayout.HelpBox("This will download all the sdks related to you project and install its dependencies", MessageType.Info);
                    }
                    else
                    {
                        if (KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName != "Unknown")
                        {
                            EditorGUILayout.HelpBox("You need to redownload the mediation !", MessageType.Error);
                        }
                    }

                    var hasSdk = KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList.Count > 0;

                    if (!hasSdk)
                    {
                        EditorGUILayout.HelpBox($"Target : {EditorUserBuildSettings.activeBuildTarget} has no sdk in its configuration", MessageType.Warning);
                    }

                    GUI.enabled = hasSdk;
                    if (GUILayout.Button(KetchappEditorUtils.SdkService.IsMediationDifferent() ? "Update Game" : "Download and Setup Game"))
                    {
                        await DownloadAllSdks();
                    }

                    GUI.enabled = true;
                    if (isDownloadingSdks)
                    {
                        var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                        EditorGUI.ProgressBar(rect, (float)sdkDownloaded / (float)sdkCount, $"{downloadingSdkName} {sdkDownloaded}/{sdkCount}");
                    }

                    KetchappEditorHelper.GuiLine(GUI.skin);
                    if (KetchappEditorUtils.SdkService.LocalSdkVersions.Count > 0)
                    {
                        KetchappEditorHelper.Label("SDKs Currently installed", 15);
                        foreach (var sdk in KetchappEditorUtils.SdkService.LocalSdkVersions)
                        {
                            KetchappEditorHelper.SDKInfoItem(sdk);
                        }

                        GUI.enabled = true;

                        GUILayout.FlexibleSpace();
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Remove third-party installation"))
                        {
                            foreach (var sdk in KetchappEditorUtils.SdkService.LocalSdkVersions)
                            {
                                KetchappEditorUtils.SdkService.RemoveSDK(sdk);
                                KetchappEditorUtils.SdkService.RemoveScriptingDefine(sdk.Type == SDKType.Mediation ? $"MEDIATION_{KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName}" : sdk.Name);
                            }

                            KetchappEditorUtils.UnityServicesManager.DisableUnityService(UnityServiceType.Purchasing);
                            KetchappEditorUtils.SdkService.RemoveAdapters();
                        }

                        GUI.backgroundColor = buttonColor;
                    }
                }
                else if (KetchappEditorUtils.Configuration.ConfigurationObject != null)
                {
                    var hasIosConfiguration = KetchappEditorUtils.Configuration.ConfigurationObject.IosConfiguration != null;
                    EditorGUILayout.HelpBox($"You have no configuration active for this target, switch to {(hasIosConfiguration ? "iOS" : "Android")}", MessageType.Warning);
                }
            }
        }

        private static async Task DownloadAllSdks()
        {
            var settings = KetchappEditorUtils.Configuration.PlatformConfiguration;
            sdkDownloaded = 0;

            isDownloadingSdks = true;
            sdkCount = settings.SdkList.Count;

            EditorApplication.LockReloadAssemblies();
            try
            {
#if UNITY_ANDROID
                KetchappEditorUtils.SdkService.SetUpAndroidManifestAndGradle();
#endif
                foreach (var sdk in settings.SdkList)
                {
                    Debug.Log("install :" + sdk.Name);
                    downloadingSdkName = sdk.Name;
                    await KetchappEditorUtils.SdkService.DownloadAndImportSDKAsync(sdk);

                    sdkDownloaded++;
                }
            }
            catch
            {
                EditorApplication.UnlockReloadAssemblies();
            }

            KetchappEditorUtils.SdkService.RemovePlayServiceResolver();
            EditorApplication.UnlockReloadAssemblies();
            KetchappEditorUtils.SdkService.RemoveAdapters();
            KetchappEditorUtils.SdkService.DownloadAdapters();
            KetchappEditorUtils.SdkService.AddScriptingDefine("MayoSDK");

            isDownloadingSdks = false;
        }

        private static ExistingConfiguration GetExistingConfiguration()
        {
            if (!KetchappEditorUtils.Configuration.ConfigurationObjectExists)
            {
                return new ExistingConfiguration()
                {
                    StudioId = 0,
                    GameId = 0
                };
            }

            var config = KetchappEditorUtils.Configuration.ConfigurationObject;
            var stdId = KetchappEditorUtils.Configuration.StudioList.FindIndex(s => s.Name == config.StudioName);

            if (stdId == -1)
            {
                KetchappEditorUtils.SdkService.RemoveKetchappConfiguration();
                return new ExistingConfiguration()
                {
                    StudioId = 0,
                    GameId = 0
                };
            }

            var gameId = KetchappEditorUtils.Configuration.StudioList[stdId].Games.FindIndex(g => g.GameName == config.GameName);

            if (gameId == -1)
            {
                KetchappEditorUtils.SdkService.RemoveKetchappConfiguration();
                return new ExistingConfiguration()
                {
                    StudioId = 0,
                    GameId = 0
                };
            }

            return new ExistingConfiguration()
            {
                StudioId = stdId,
                GameId = gameId
            };
        }

        private static async Task RetrieveConfigurationObject()
        {
            var selectedGame = KetchappEditorUtils.Configuration.StudioList[_selectedStudioId].Games[_selectedGameId].Id;
            GameDetailDto details = new GameDetailDto();
            try
            {
                details = await KetchappEditorUtils.MayoApiClient.GetGameDetailAsync(selectedGame, CancellationToken.None);
            }
            catch (Exception e)
            {
                isFetchingConfig = false;
                Debug.LogError(e);
            }

            KetchappEditorUtils.Configuration.CreateConfigurationObjectAsync(details, KetchappEditorUtils.Configuration.UserStudios[_selectedStudioId]);
        }

        private static void CheckUnityVersionsCompatibility(out string versionNumber)
        {
            var currentVersion = UnityVersion.Parse(Application.unityVersion);

            var maxUnRecommendedVersion = KetchappEditorUtils.Configuration.ConfigurationObject.UnRecommendedUnityVersions?
                .FirstOrDefault(x => currentVersion.CompareTo(UnityVersion.Parse(x.Min)) >= 0 && currentVersion.CompareTo(UnityVersion.Parse(x.Max)) < 0)?.Max;

            versionNumber = maxUnRecommendedVersion;
        }
    }
}
