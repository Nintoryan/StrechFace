using System;
using UnityEngine;

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