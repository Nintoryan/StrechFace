using System;
using System.IO;
using Ketchapp.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KetchappCustomConstants))]
public class KetchappCustomConstantsEditor : Editor
{
    [MenuItem("Ketchapp Mayo SDK/Ketchapp Constants")]
    public static void OpenEditor()
    {
        Selection.activeObject = KetchappCustomConstantsBuilder.Build();
    }

    public override void OnInspectorGUI()
    {
        var settings = (KetchappCustomConstants)target;
        GUILayout.Box(KetchappEditorHelper.KetchappLogo, GUILayout.Height(100), GUILayout.Width(Screen.width));
        KetchappEditorHelper.GuiLine(GUI.skin);
        EditorGUILayout.HelpBox("Here, you can set the constants value used by the Mayo SDK", MessageType.Info);

        EditorGUILayout.BeginVertical(GUI.skin.GetStyle("HelpBox"));
        settings.BannerHeight = EditorGUILayout.IntField("Height of banner background", settings.BannerHeight);
        settings.InterstitialDelay = EditorGUILayout.IntField("Minimal time between showing two interstitial", settings.InterstitialDelay);
        settings.AppsflyerCustomKey = EditorGUILayout.TextField("Appsflyer Custom API Key", settings.AppsflyerCustomKey);
        EditorGUILayout.EndVertical();

        EditorUtility.SetDirty(settings);
    }
}
