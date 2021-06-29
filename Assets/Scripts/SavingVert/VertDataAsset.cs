using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVertData.asset", menuName = "Custom Data/VertData")]
public class VertDataAsset : ScriptableObject
{
    public VertLevelData data;

#if UNITY_EDITOR
    public TextAsset sourcePath;
    public void Import()
    {
        data = JsonUtility.FromJson<VertLevelData>(sourcePath.text);
        EditorUtility.SetDirty(this);
    }
#endif
}