#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StcrechingFace.Tool
{
    public static class ReferenceVertSaver
    {
        [MenuItem("HerbariumGames/SaveReference")]
        private static void NewMenuOption()
        {
            var refer = GameObject.FindWithTag("Streching").GetComponent<MeshFilter>();
            var _levelNumber = SceneManager.GetActiveScene().buildIndex;
            var sv = new VertLevelData(refer.sharedMesh.vertices,_levelNumber);
            Debug.Log(refer.sharedMesh.vertices.Length);
            var jsonSv = JsonUtility.ToJson(sv);
            var path = Application.dataPath;
            File.WriteAllText(path+$"/Scripts/SavingVert/ReferencedJsons/{_levelNumber}.json",jsonSv);
        }
    }

    public static class PPdeleter
    {
        [MenuItem("HerbariumGames/DeletePP")]
        private static void NewMenuOption()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif

[Serializable]
public class VertLevelData
{
    public Vector3[] Vertecies;
    public int LevelNumber;

    public VertLevelData(Vector3[] _vertecies, int _levelNumber)
    {
        Vertecies = _vertecies;
        LevelNumber = _levelNumber;
    }
}