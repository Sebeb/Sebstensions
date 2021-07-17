using System.Collections.Generic;
using System.Reflection;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MonoBehaviour), true)]
public class CustomMonoEditor : Editor
{
    private SerializedObject so;
    private HashSet<SerializedProperty> settingsSPs;

    protected virtual void OnEnable()
    {
        settingsSPs = new HashSet<SerializedProperty>();
        foreach (PropertyInfo setting in
            ReflectionUtility.GetAllProperties(
                target, p => p. GetCustomAttributes(typeof(BindSettingAttribute), true).Length > 0))
        {
            SettingsBox box = (SettingsBox)setting.GetValue(target);
            if (box == null) { continue; }
            #if UNITY_EDITOR
            if (box.parentScript == null)
            {
                box.parentScript = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
            }
            #endif

            so = new SerializedObject(SettingsManager._i);

            for (int i = 0; i < SettingsManager._i.boxes.Count; i++)
            {
                if (SettingsManager._i.boxes[i] != box) { continue; }

                settingsSPs.Add(so.FindProperty($"boxes.Array.data[{i}]"));
            }
        }
    }

    public override void OnInspectorGUI()
    {
        foreach (SerializedProperty sp in settingsSPs)
        {
            EditorGUILayout.PropertyField(sp, true);
        }
        if (settingsSPs.Count > 0) { so.ApplyModifiedProperties(); }

        DrawDefaultInspector();
    }
}