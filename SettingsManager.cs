using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public static class SettingsManager 
{
	private static Dictionary<Type, SettingsScriptable> boxDic;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), InitializeOnLoadMethod]
	public static void OnScriptReload()
	{
		PopulateDic();
		foreach (Type newType in ScriptablesDatabase.Get<SettingsScriptable>()
			         .Select(s => s.settings.GetType())
			         .Except<Type>(boxDic.Keys))
		{
			SettingsScriptable scriptable = ScriptableMonoObject.CreateNew<SettingsScriptable>(newType.Name.NormalizeCamel()) as SettingsScriptable;
			scriptable.settings = Activator.CreateInstance(newType) as Settings;
		}
	}

	private static void PopulateDic()
	{
		boxDic = new Dictionary<Type, SettingsScriptable>(ScriptablesDatabase.Get(typeof(SettingsScriptable))
			.Select(t =>
				new KeyValuePair<Type, SettingsScriptable>(((SettingsScriptable)t).settings.GetType(),
					(SettingsScriptable)t)));
	}

	public static T1 GetSetting<T1>() where T1 : Settings, new()
	{
		if (boxDic is null || boxDic.Count == 0 || Application.isEditor && !Application.isPlaying)
		{
			PopulateDic();
		}

		if (boxDic.TryGetValue(typeof(T1), out SettingsScriptable existingBox)) return existingBox.settings as T1;

		Debug.LogError($"No settings of type {typeof(T1)} found");

		return null;
	}
}