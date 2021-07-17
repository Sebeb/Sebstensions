using System;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public abstract class SettingsBox
{
    [HideInInspector]
    public string name;
    [ReadOnly]
    public MonoScript parentScript;

    public SettingsBox()
    {
        name = GetType().ToString().NormalizeCamel();
    }
}