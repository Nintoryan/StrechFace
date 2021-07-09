#if UNITY_EDITOR
using System.IO;
using System.Net.Http;
using Ketchapp.Editor.Purchasing;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor.Utils
{
    [InitializeOnLoad]
    internal class TokenRemover : EditorWindow
    {
        static TokenRemover()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.timeSinceStartup <= 20)
            {
                FileUtil.DeleteFileOrDirectory(KetchappEditorUtils.AuthenticationManager.TokenPath);
            }

            if (!File.Exists(IAPEditor.PurchasingEnumPath))
            {
                IAPEditor.GenerateDefaultPurchasingFile();
            }

            Application.logMessageReceived += HandleMayoEditorLog;
        }

        private static void HandleMayoEditorLog(string condition, string stackTrace, LogType logType)
        {
            if (logType == LogType.Exception)
            {
                if (Application.isEditor && !Application.isPlaying)
                {
                    if (condition.Contains(nameof(HttpRequestException)) && condition.Contains("Unauthorized"))
                    {
                        FileUtil.DeleteFileOrDirectory(KetchappEditorUtils.AuthenticationManager.TokenPath);
                    }
                }
            }
        }
    }
}
#endif