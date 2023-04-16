using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable]
public abstract class SettingsBox
{
    [HideInInspector]
    public string name;
#if UNITY_EDITOR
    [ReadOnly]
    public MonoScript parentScript;
#endif

    public SettingsBox()
    {
        name = GetType().ToString().NormalizeCamel();
    }
}