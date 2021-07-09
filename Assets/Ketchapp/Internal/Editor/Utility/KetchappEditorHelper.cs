using System.Collections.Generic;
using System.Linq;
using Ketchapp.Editor.Utils;
using Ketchapp.Internal.Configuration;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor
{
    internal static class KetchappEditorHelper
    {
        public static bool IsLogging { get; set; }
        public static Texture2D KetchappLogo { get; set; }
        public static Texture2D ISLogo { get; set; }
        public static Texture2D AppLovinLogo { get; set; }
        public static Texture2D FacebookLogo { get; set; }
        public static Texture2D DollarIcon { get; set; }

        private const string ImagePath = "Assets/Ketchapp/Editor/Images";

        private static Texture2D UninstallIcon { get; set; }
        private static GUIStyle _uninstallButtonStyle = new GUIStyle(EditorStyles.miniButton)
        {
            fixedWidth = 18,
            fixedHeight = 18,
            padding = new RectOffset(1, 1, 1, 1)
        };

        static KetchappEditorHelper()
        {
            KetchappLogo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ImagePath}/Ketchapp_Logo.png");
            ISLogo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ImagePath}/IS_Banner.png");
            AppLovinLogo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ImagePath}/AppLovin_Banner.png");
            FacebookLogo = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ImagePath}/Facebook.png");
            UninstallIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ImagePath}/uninstall_icon.png");
            DollarIcon = AssetDatabase.LoadAssetAtPath<Texture2D>($"{ImagePath}/dollar-symbol.png");
        }

        public static void GuiLine(GUISkin skin, int i_height = 1)
        {
            EditorGUILayout.LabelField(string.Empty, skin.horizontalSlider);
        }

        public static void LoadingText()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(EditorGUIUtility.IconContent("d_WaitSpin05"));
            Label("Loading data");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        public static void Label(string text, int size = 12, int width = 0)
        {
            var style = GUI.skin.GetStyle("label");
            var tempSize = style.fontSize;
            style.normal.textColor = Color.white;
            style.fontSize = size;
            style.fixedWidth = width;

            GUILayout.Label(text, style);
            style.fontSize = tempSize;
            style.fixedWidth = 0;
        }

        public static void Label(string text, Color textColor, int size = 12, int width = 0)
        {
            var style = GUI.skin.GetStyle("label");
            var tempSize = style.fontSize;
            style.normal.textColor = textColor;
            style.fixedWidth = width;
            style.fontSize = size;
            GUILayout.Label(text, style);
            style.fontSize = tempSize;
            style.fixedWidth = 0;
        }

        public static async void SdkUpdateLine(SDKVersion sdk)
        {
            bool isLoading = false;
            EditorGUIUtility.SetIconSize(Vector2.zero);
            var rect = EditorGUILayout.BeginHorizontal(GUI.skin.GetStyle("HelpBox"));
            Label(sdk.Name, width: 150);
            Label(sdk.Version, width: 50);

            if (!KetchappEditorUtils.SdkService.IsPackageInstalled(sdk))
            {
                Label("Not installed", textColor: Color.red, width: 100);
                GUI.enabled = !isLoading;
                if (GUILayout.Button("Install"))
                {
                    isLoading = true;
                    await KetchappEditorUtils.SdkService.DownloadAndImportSDKAsync(sdk);
                    isLoading = false;
                }

                GUI.enabled = true;
            }
            else
            {
                if (KetchappEditorUtils.SdkService.LocalSdkHasUpdateAsync(sdk))
                {
                    Label("Update available !", Color.yellow, width: 100);
                    GUILayout.ExpandWidth(true);
                    if (GUILayout.Button("Update"))
                    {
                        await KetchappEditorUtils.SdkService.DownloadAndImportSDKAsync(sdk);
                    }
                }
                else
                {
                    Label("Up-to-date", Color.green, width: 100);
                }

                if (GUILayout.Button("Remove"))
                {
                    KetchappEditorUtils.SdkService.RemoveSDK(sdk);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void SDKInfoItem(SDKVersion sdk)
        {
            var rect = EditorGUILayout.BeginHorizontal(GUI.skin.GetStyle("HelpBox"));
            Label(sdk.Name, width: 150);
            GUILayout.FlexibleSpace();
            if (sdk.PackageType == MayoAPI.Dto.PackageType.UnityPackageRegistry)
            {
                Label("Managed by UPM", width: 150);
            }
            else
            {
                Label("Unity Package", width: 150);
            }

            Label(sdk.Version, width: 100);
            EditorGUILayout.EndHorizontal();
        }

        public static (GUIContent[] guiContent, TabType[] tabs) GetTabItems(bool hasNewVersion, bool onlySetup)
        {
            var content = new List<GUIContent>();
            var tabs = new List<TabType>();

            if (hasNewVersion)
            {
                tabs.Add(TabType.MayoSDK);
                content.Add(new GUIContent()
                {
                    text = "Ketchapp Mayo SDK"
                });
            }
            else if (onlySetup)
            {
                tabs.Add(TabType.Setup);
                content.Add(new GUIContent()
                {
                    image = EditorGUIUtility.IconContent("TestPassed").image,
                    text = "Setup"
                });
            }
            else
            {
                tabs.Add(TabType.Setup);
                content.Add(new GUIContent()
                {
                    image = EditorGUIUtility.IconContent("TestPassed").image,
                    text = "Setup"
                });
                if (KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList.Any(s => s.Type == SDKType.Mediation))
                {
                    content.Add(new GUIContent()
                    {
                        image = !KetchappEditorUtils.SdkService.SdkCategoryHasUpdate(SDKType.Mediation) ? EditorGUIUtility.IconContent("TestPassed").image : EditorGUIUtility.IconContent("TestInconclusive").image,
                        text = "Ads"
                    });
                    tabs.Add(TabType.Ads);
                }

                if (KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList.Any(s => s.Type == SDKType.Analytics))
                {
                    content.Add(new GUIContent()
                    {
                        image = !KetchappEditorUtils.SdkService.SdkCategoryHasUpdate(SDKType.Analytics) ? EditorGUIUtility.IconContent("TestPassed").image : EditorGUIUtility.IconContent("TestInconclusive").image,
                        text = "Analytics"
                    });
                    tabs.Add(TabType.Analytics);
                }

                if (KetchappEditorUtils.Configuration.PlatformConfiguration.SdkList.Any(s => s.Type == SDKType.Misc))
                {
                    content.Add(new GUIContent()
                    {
                        image = !KetchappEditorUtils.SdkService.SdkCategoryHasUpdate(SDKType.Misc) ? EditorGUIUtility.IconContent("TestPassed").image : EditorGUIUtility.IconContent("TestInconclusive").image,
                        text = "Misc"
                    });
                    tabs.Add(TabType.Misc);
                }
            }

            return (content.ToArray(), tabs.ToArray());
        }
    }
}
