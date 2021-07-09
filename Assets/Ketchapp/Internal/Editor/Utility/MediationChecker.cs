using System;
using Ketchapp.Editor;
using Ketchapp.Editor.Utils;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Ketchapp.Internal.Editor
{
    internal class MediationChecker : IActiveBuildTargetChanged
    {
        public int callbackOrder => 0;

        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            if (KetchappEditorUtils.SdkService.IsMediationDifferent())
            {
                EditorUtility.DisplayDialog("Mediation update", "You need to redownload the mediation sdk", "Ok");
                KetchappAutoUpdater.ShowWindow();
            }
        }
    }
}
