using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[Serializable, InlineProperty, HideLabel]
public class Settings
{
    [HideInInspector]
    public string name;

    public Settings()
    {
        name = GetType().ToString().NormalizeCamel();
    }
}