using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ketchapp.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(XcodePlistStrings))]
public class XcodePlistStringsEditor : Editor
{
    public const string AssetPath = "Assets/Ketchapp/Internal/BuildUtil/Localization/iOS/XcodePlistLocalization/LocalizedStrings.asset";

    public List<bool> LocalizedFoldout = new List<bool>();
    private LocalizationCode _addingLocalization;

    public void OnEnable()
    {
        var settings = (XcodePlistStrings)target;
        LocalizedFoldout = new List<bool>();
        settings.Properties.ForEach(x => LocalizedFoldout.Add(false));
    }

    public override void OnInspectorGUI()
    {
        var baseBGColor = GUI.backgroundColor;
        KetchappEditorHelper.Label("xCode Plist Localization", 24);
        KetchappEditorHelper.GuiLine(GUI.skin, 2);
        var settings = (XcodePlistStrings)target;

        for (var i = 0; i < settings.Properties.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.GetStyle("Helpbox"));
            var rect = EditorGUILayout.BeginHorizontal(GUI.skin.box);
            LocalizedFoldout[i] = EditorGUILayout.Foldout(LocalizedFoldout[i], string.Empty, GetFoldoutStyle());
            settings.Properties[i].LocalizationCode = (LocalizationCode)EditorGUILayout.EnumPopup("Localization Code", settings.Properties[i].LocalizationCode);
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X"))
            {
                settings.Properties.Remove(settings.Properties[i]);
                continue;
            }

            GUI.backgroundColor = baseBGColor;

            EditorGUILayout.EndHorizontal();

            if (LocalizedFoldout[i])
            {
                for (var u = 0; u < settings.Properties[i].Properties.Count; u++)
                {
                    EditorGUILayout.BeginHorizontal(GUI.skin.box);
                    EditorGUILayout.BeginVertical();
                    settings.Properties[i].Properties[u].Property = EditorGUILayout.TextField("Plist Property Name", settings.Properties[i].Properties[u].Property);
                    settings.Properties[i].Properties[u].Value = EditorGUILayout.TextField("Localized Value", settings.Properties[i].Properties[u].Value);

                    EditorGUILayout.EndVertical();
                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        settings.Properties[i].Properties.Remove(settings.Properties[i].Properties[u]);
                    }

                    GUI.backgroundColor = baseBGColor;

                    EditorGUILayout.EndHorizontal();
                }
            }

            if (GUILayout.Button("Add"))
            {
                settings.Properties[i].Properties.Add(new PlistProperty());
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        KetchappEditorHelper.GuiLine(GUI.skin);

        if (settings.Properties.Count < Enum.GetNames(typeof(LocalizationCode)).Count())
        {
            EditorGUILayout.BeginHorizontal();
            _addingLocalization = (LocalizationCode)EditorGUILayout.EnumPopup("Locale", _addingLocalization);
            var exists = settings.Properties.Any(p => p.LocalizationCode == _addingLocalization);
            GUI.enabled = !exists;
            if (GUILayout.Button("Add"))
            {
                settings.Properties.Add(new LocalizedProperty(_addingLocalization));
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            if (exists)
            {
                EditorGUILayout.HelpBox("Setting with this Localization Code already exists", MessageType.Error);
            }
        }

        if (settings.Properties.Count == 0)
        {
            if (GUILayout.Button("Add All"))
            {
                foreach (LocalizationCode code in Enum.GetValues(typeof(LocalizationCode)))
                {
                    settings.Properties.Add(new LocalizedProperty(code));
                }
            }
        }

        EditorUtility.SetDirty(target);
    }

    private static GUIStyle GetFoldoutStyle()
    {
        GUIStyle style = new GUIStyle(EditorStyles.foldout);
        style.fixedWidth = 0;
        return style;
    }

    [MenuItem("Assets/Create/Ketchapp/Localization/iOS/Xcode/Create")]
    public static void CreateAsset()
    {
        XcodePlistStrings asset = ScriptableObject.CreateInstance<XcodePlistStrings>();
        AssetDatabase.CreateAsset(asset, AssetPath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
