using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Weaver;

[DefaultExecutionOrder(-999)]
public class SettingsManager : SingletonScriptableObject<SettingsManager>
{
    [SerializeReference]
    public List<SettingsBox> boxes = new List<SettingsBox>();
    public static Dictionary<Type, dynamic> boxDic;


    private static void PopulateDicFromList()
    {
        OrganiseList();
        boxDic = new Dictionary<Type, dynamic>();
        foreach (SettingsBox settingsBox in _i.boxes)
        {
            boxDic.Add(settingsBox.GetType(), settingsBox);
        }

        Debug.Log($"Loaded {_i.boxes.Count} sets of settings");
    }

    public static T1 GetSetting<T1>() where T1 : SettingsBox, new()
    {
        if (boxDic == null) { PopulateDicFromList(); }

        boxDic.TryGetValue(typeof(T1), out dynamic existingBox);

        if (existingBox != null) { return existingBox as T1; }

        existingBox = _i.boxes.FirstOrDefault(b => b.GetType() == typeof(T1));

        if (existingBox != null)
        {
            boxDic[typeof(T1)] = existingBox;
            return existingBox as T1;
        }

        SettingsBox box = new T1();
        Debug.Log($"Made new {box.GetType().ToString().NormalizeCamel()}");
        boxDic[typeof(T1)] = box;
        _i.boxes.Add(box);

        return box as T1;
    }

    private static void OrganiseList()
    {
        _i.boxes.RemoveAll(b => b == null);
        _i.boxes.Sort((a, b) => a.name.CompareTo(b.name));
    }

}