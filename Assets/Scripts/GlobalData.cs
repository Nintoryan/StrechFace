using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData
{
    public static int ProgressLevel
    {
        get => PlayerPrefs.GetInt("CurrentLevel");
        set => PlayerPrefs.SetInt("CurrentLevel", value);
    }
    public static int LoadableLevel
    {
        get => PlayerPrefs.GetInt("LoadableLevel");
        set => PlayerPrefs.SetInt("LoadableLevel", value);
    }
}
