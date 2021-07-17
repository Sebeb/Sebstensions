using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Weaver;
using Object = UnityEngine.Object;

public class SettingsManager : SingletonScriptableObject<SettingsManager>
{
    [SerializeReference, NonReorderable]
    public List<SettingsBox> boxes = new List<SettingsBox>();
    public static Bictionary<Type, HashSet<SettingsBox>> boxDic
        = new Bictionary<Type, HashSet<SettingsBox>>();

    public static T1 BindSetting<T1, T2>() where T1 : SettingsBox, new() where T2 : MonoBehaviour
        => Bind(new T1(), typeof(T2)) as T1;

    public static SettingsBox Bind(SettingsBox box, Type objectType)
    {
        SettingsBox existingBox = _i.boxes.FirstOrDefault(b => b.GetType() == box.GetType());

        //Remove the dictionary entry for the old owner, if an entry containing this type already
        //exists and use the pre-existing box instead
        if (existingBox != null)
        {
            Type previousOwner
                = boxDic.Keys.FirstOrDefault(k => boxDic[k].Contains(existingBox));
            if (previousOwner != objectType)
            {
                if (previousOwner != null)
                {
                    Debug.Log(
                        $"Switching binding for {box.name} from {previousOwner} to {objectType}");
                    boxDic[previousOwner].Remove(existingBox);
                }
            }
            box = existingBox;
        }
        else
        {
            _i.boxes.Add(box);
            Debug.Log($"Added new game settings: {box.name}");
            Refresh();
        }

        if (!boxDic.ContainsKey(objectType))
        {
            boxDic.Add(objectType, new HashSet<SettingsBox>());
        }

        boxDic[objectType].Add(box);

        box.parentScript = MonoScript.FromMonoBehaviour((MonoBehaviour)FindObjectOfType(objectType));

        return box;
    }

    private static void Refresh()
    {
        _i.boxes.RemoveAll(b => b == null);
        _i.boxes.Sort((a, b) => a.name.CompareTo(b.name));
    }

    public static SettingsBox[] GetSettingsBoxes(Type mono) => boxDic[mono]?.ToArray();

    public static SettingsBox GetSettings(SettingsBox s) => GetSettings(s.GetType());
    public static SettingsBox GetSettings(Type t) =>
        _i.boxes.FirstOrDefault(b => b.GetType() == t);
    public static SettingsBox GetSettings<T>() where T : SettingsBox
        => _i.boxes.FirstOrDefault(b => b.GetType() == typeof(T));


}