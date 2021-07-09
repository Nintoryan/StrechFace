using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ketchapp.Editor.Utils;
using Ketchapp.Internal.Configuration;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor
{
    internal class KetchappAutoUpdater : AuthorizedWindow
    {
        public SDKVersion NewVersion { get; set; }
        public bool HasNewVersion
        {
            get
            {
                if (NewVersion == null)
                {
                    return false;
                }

                return NewVersion.Version != KetchappEditorUtils.SdkService.KetchappSDKVersion;
            }
        }

        public bool IsDownloadingSDK { get; set; }

        public bool IsRemovingSDK { get; set; }

        public GameInfos GameConfiguration { get; set; }

        public int CurrentTab { get; set; }

        private int _selectedMediationIndex;
        private bool HasCheckedVersion;
        private string[] _availableMediations;

        private bool ConfigurationChecked;
        public static KetchappAutoUpdater WindowReference { get; set; }

        private GUISkin CustomSkin { get; set; }

        [MenuItem("Ketchapp Mayo SDK/Setup")]
        public static void ShowWindow()
        {
            WindowReference = (KetchappAutoUpdater)GetWindow<KetchappAutoUpdater>("Setup Ketchapp Mayo SDK", Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll"));
            KetchappConfigurationWindow.ConfigInitialized = false;
        }

        public void CloseWindow()
        {
            Close();
        }

        private void OnEnable()
        {
        }

        private async Task FetchMainSdkInfoAsync()
        {
                var result = await KetchappEditorUtils.SdkService.HasNewMainSDKVersionAsync();
                NewVersion = result;
        }

        private async Task CheckForUpdate()
        {
            await FetchMainSdkInfoAsync();
            ConfigurationChecked = true;
        }

        public override async void OnGUI()
        {
            base.OnGUI();
            EditorStyles.textField.wordWrap = true;
            if (KetchappEditorUtils.AuthenticationManager.IsUserAuthenticated)
            {
                if (!ConfigurationChecked)
                {
                    await CheckForUpdate();
                    return;
                }

                KetchappEditorHelper.GuiLine(GUI.skin);
                KetchappEditorHelper.Label("Ketchapp Mayo SDK Updater", 20);
                KetchappEditorHelper.Label($"v{KetchappEditorUtils.SdkService.KetchappSDKVersion}");
                KetchappEditorHelper.GuiLine(GUI.skin);

                var shouldShowOnlySetup = !(KetchappEditorUtils.Configuration.ConfigurationObjectExists && KetchappEditorUtils.SdkService.LocalSdkVersions.Count > 0 && !KetchappEditorUtils.SdkService.IsMediationDifferent());
                CurrentTab = GUILayout.Toolbar((int)CurrentTab, KetchappEditorHelper.GetTabItems(HasNewVersion, shouldShowOnlySetup).guiContent);

                switch (KetchappEditorHelper.GetTabItems(HasNewVersion, shouldShowOnlySetup).tabs[CurrentTab])
                {
                    case TabType.Setup:
                        KetchappConfigurationWindow.ShowConfigurationPanel();
                        break;
                    case TabType.MayoSDK:
                        {
                            KetchappEditorHelper.Label($"Current version : {KetchappEditorUtils.SdkService.KetchappSDKVersion}");
                            KetchappEditorHelper.GuiLine(GUI.skin);

                            if (HasNewVersion && NewVersion != null)
                            {
                                KetchappEditorHelper.Label($"New version : {NewVersion.Version}", Color.green);
                                EditorGUILayout.TextArea(NewVersion.Changelog);

                                if (GUILayout.Button("Download"))
                                {
                                    var newSdk = await KetchappEditorUtils.MayoApiClient.GetMayoSDKVersionAsync(CancellationToken.None);
                                    await KetchappEditorUtils.SdkService.DownloadAndImportSDKAsync(newSdk.MapFromDto());
                                    await CheckForUpdate();
                                    return;
                                }
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("Mayo SDK is up-to-date !", MessageType.Info);
                            }

                            break;
                        }

                    case TabType.Ads:
                        {
                            if (KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName == string.Empty)
                            {
                                EditorGUILayout.HelpBox("Fetch your configuration to download the mediation SDK", MessageType.Warning);
                                if (GUILayout.Button("Show Configuration window"))
                                {
                                }

                                break;
                            }

                            bool mediationInstalled = KetchappEditorUtils.SdkService.LocalSdkVersions.Any(s => s.Type == SDKType.Mediation && KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName == s.Name);

                            GUI.enabled = !mediationInstalled;

                            KetchappEditorHelper.Label($"Current Mediation : {KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName}", 13);
                            KetchappEditorHelper.GuiLine(GUI.skin);
                            GUI.enabled = true;

                            if (mediationInstalled)
                            {
                                KetchappEditorHelper.SdkUpdateLine(KetchappEditorUtils.SdkService.LocalSdkVersions.FirstOrDefault(s => s.Name == KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName));
                            }
                            else
                            {
                                GUI.enabled = !IsDownloadingSDK;
                                if (GUILayout.Button(IsDownloadingSDK ? "Downloading .." : "Download Mediation SDK"))
                                {
                                    EditorApplication.LockReloadAssemblies();
                                    IsDownloadingSDK = true;

                                    KetchappEditorUtils.SdkService.CheckForExistingMediation();

                                    await KetchappEditorUtils.SdkService.DownloadAndImportSDKAsync(
                                        KetchappEditorUtils.SdkService.DistantSdkVersions.FirstOrDefault(s =>
                                            s.Name == KetchappEditorUtils.Configuration.PlatformConfiguration.MediationName));

                                    IsDownloadingSDK = false;
                                    EditorApplication.UnlockReloadAssemblies();
                                    EditorUtility.DisplayDialog("Action required", "You need to restart the editor in order to complete the installation", "Ok");
                                }

                                GUI.enabled = true;
                            }

                            if (GUILayout.Button("Install Adapters"))
                            {
                                KetchappEditorUtils.SdkService.RemoveAdapters();
                                KetchappEditorUtils.SdkService.DownloadAdapters();
                            }

                            break;
                        }

                    case TabType.Analytics:
                        {
                            foreach (SDKVersion sdk in KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList)
                            {
                                if (sdk.Type == SDKType.Analytics)
                                {
                                    KetchappEditorHelper.SdkUpdateLine(sdk);
                                }
                            }

                            break;
                        }

                    case TabType.Misc:
                        {
                            foreach (SDKVersion sdk in KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList)
                            {
                                if (sdk.Type == SDKType.Misc)
                                {
                                    KetchappEditorHelper.SdkUpdateLine(sdk);
                                }
                            }

                            break;
                        }

                    default:
                        break;
                }
            }
        }
    }

    public enum TabType
    {
        Setup,
        MayoSDK,
        Ads,
        Analytics,
        Misc
    }
}
