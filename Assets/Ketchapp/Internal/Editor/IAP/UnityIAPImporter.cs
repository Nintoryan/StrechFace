using System.Collections;
using Ketchapp.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Ketchapp.Editor.Utils
{
    public static class UnityIAPImporter
    {
        public static void ImportUnityIAP()
        {
            Debug.Log(EditorConstants.UnityIAPPath);
            AssetDatabase.ImportPackage(EditorConstants.UnityIAPPath, false);
        }
    }
}
