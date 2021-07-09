using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class XcodePlistStrings : ScriptableObject
{
    public List<LocalizedProperty> Properties;
}

[Serializable]
public class LocalizedProperty
{
    public LocalizedProperty(LocalizationCode code)
    {
        LocalizationCode = code;
        Properties = new List<PlistProperty>();
    }

    public LocalizationCode LocalizationCode;
    public List<PlistProperty> Properties;
}

[Serializable]
public class PlistProperty
{
    public string Property;
    public string Value;
}

public enum LocalizationCode
{
    EN,
    CN,
    FR,
    DE,
    JA,
    KO,
    ES,
    IT,
    RU,
    AR,
    PT,
    HI,
    BN,
    KR
}
