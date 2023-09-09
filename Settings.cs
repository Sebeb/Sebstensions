using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public class Settings<T> : Settings where T : class {}

[Serializable, InlineProperty, HideLabel]
public class Settings
{
	[ClearOnReload]
	private static Dictionary<Type, SettingsScriptable> classTypeDic, settingTypeDic;

	[InitializeOnLoadMethod, MenuItem("Tools/Reload Settings"), ExecuteOnReload]
	public static void OnScriptReload()
	{
		PopulateDic();
		bool hasChanged = false;
		IEnumerable<Type> settingTypes = Reflection.GetAllScriptChildTypes<Settings>().Except(settingTypeDic.Keys);
		foreach (Type newType in settingTypes)
		{
			SettingsScriptable scriptable =
				ScriptableMonoObject.CreateNew<SettingsScriptable>(newType.Name.NormalizeCamel());
			scriptable.settings = Activator.CreateInstance(newType) as Settings;

			hasChanged = true;
			EditorUtility.SetDirty(scriptable);
		}

		if (hasChanged)
		{
			PopulateDic();
		}
	}

	private static void TryPopulateDic()
	{
		if (settingTypeDic is null || settingTypeDic.Count == 0)
		{
			PopulateDic();
		}
	}


	private static void PopulateDic()
	{
		ScriptablesDatabase.TryRefresh();
		settingTypeDic = new Dictionary<Type, SettingsScriptable>(ScriptablesDatabase.Get(typeof(SettingsScriptable))
			.Select(t =>
			{
				if (t is not SettingsScriptable s)
				{
					Debug.LogError($"Error with settings");

					return default;
				}

				if (s is not { settings: not null })
				{
					Debug.LogError($"Null setting on {AssetDatabase.GetAssetPath(t)}");
					return default;
				}

				return new KeyValuePair<Type, SettingsScriptable>(s.settings.GetType(), s);
			}).Where(s => s.Value != null));
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

	public static SettingsScriptable GetBox(Type type, bool targetClassType, bool silent = false)
	{
		TryPopulateDic();

		Dictionary<Type, SettingsScriptable> dic = targetClassType ? classTypeDic : settingTypeDic;
		if (dic.TryGetValue(type, out SettingsScriptable existingBox)) return existingBox;

		if (!silent) Debug.LogError($"No settings of type {type} found");

		return null;
	}

	public object Select(Func<object, object> func) { throw new NotImplementedException(); }
}