using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt("CurrentLevel");
        set => PlayerPrefs.SetInt("CurrentLevel", value);
    }
}
