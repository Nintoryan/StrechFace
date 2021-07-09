using System;
using Ketchapp.Editor.Utils;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal class MigrationApplication : Editor
{
    static MigrationApplication()
    {
        if (PlayerPrefs.GetString(KetchappEditorUtils.MigrationManager.SavedLastMigrationVersionKey, string.Empty) != KetchappEditorUtils.SdkService.KetchappSDKVersion)
        {
            Debug.Log("Launching migration");
            KetchappEditorUtils.MigrationManager.Migrate();
        }
    }
}
