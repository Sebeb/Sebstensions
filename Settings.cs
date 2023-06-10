using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public class ClassSettings<T> : Settings where T : CustomMono {}

[Serializable, InlineProperty, HideLabel]
public class Settings
{
	private static Dictionary<Type, SettingsScriptable> classTypeDic, settingTypeDic;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), InitializeOnLoadMethod]
	public static void OnScriptReload()
	{
		PopulateDic();
		foreach (Type newType in ScriptablesDatabase.Get<SettingsScriptable>()
			         .Select(s => s.settings.GetType())
			         .Except<Type>(classTypeDic.Keys))
		{
			SettingsScriptable scriptable =
				ScriptableMonoObject.CreateNew<SettingsScriptable>(newType.Name.NormalizeCamel()) as SettingsScriptable;
			scriptable.settings = Activator.CreateInstance(newType) as Settings;
		}
	}

	private static void TryPopulateDic()
	{
		if (settingTypeDic is null || settingTypeDic.Count == 0 || Application.isEditor && !Application.isPlaying)
		{
			PopulateDic();
		}
	}

	private static void PopulateDic()
	{
		settingTypeDic = new Dictionary<Type, SettingsScriptable>(ScriptablesDatabase.Get(typeof(SettingsScriptable))
			.Select(t => new KeyValuePair<Type, SettingsScriptable>(
				((SettingsScriptable)t).settings.GetType(),
				(SettingsScriptable)t)));
		classTypeDic = settingTypeDic.ToDictionary(kvp => GetAssociatedType(kvp.Value.settings), kvp => kvp.Value);
	}

	public static Type GetAssociatedType(Settings settings) =>
		settings.GetType().BaseType.GetGenericArguments()[0];

	public static T Get<T>() where T : Settings => Get(typeof(T), false) as T;

	public static Settings Get(Type type, bool targetClassType)
	{
		TryPopulateDic();

		if (GetBox(type, targetClassType) is SettingsScriptable existingBox) return existingBox.settings;

		Debug.LogError($"No settings of type {type} found");

		return null;
	}

	public static SettingsScriptable GetBox(Type type, bool targetClassType)
	{
		TryPopulateDic();
		
		Dictionary<Type, SettingsScriptable> dic = targetClassType ? classTypeDic : settingTypeDic;
		if (dic.TryGetValue(type, out SettingsScriptable existingBox)) return existingBox;

		Debug.LogError($"No settings of type {type} found");

		return null;
	}

	public object Select(Func<object, object> func) { throw new NotImplementedException(); }
}