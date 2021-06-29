using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VertDataAsset))]
public class VertDataEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector ();
        
        var asset = (VertDataAsset)target;
        
        if(GUILayout.Button(asset.data == null ? "Import" : "Reimport")){

            Undo.RecordObject(asset, "Import JSON");
            
            asset.Import();
        }
    }
}