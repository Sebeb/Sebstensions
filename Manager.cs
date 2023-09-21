using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;



public static class Managers
{
	[ClearOnReload]
	private static Dictionary<Type, ScriptableMonoObject> associatedTypeSetting;

	public static ScriptableMonoObject Get(Type type) => ScriptablesDatabase.Get(type).FirstOrDefault();

	public static ScriptableMonoObject GetAssociated(Type type, bool silent = false)
	{
		if (associatedTypeSetting.TryGetValue(type, out ScriptableMonoObject manager)) return manager;

		if (!silent) Debug.LogError($"No manager of type {type} found");

		return null;
	}

#if UNITY_EDITOR
	[InitializeOnLoadMethod,
 #else
[
 #endif
	 ExecuteOnReload]
	private static void PopulateAssociationDic()
	{
		ScriptablesDatabase.TryRefresh();
		associatedTypeSetting = new Dictionary<Type, ScriptableMonoObject>(ScriptablesDatabase
			.Get(typeof(Manager<>))
			.Select(t =>
			{
				if (t is not {} s)
				{
					Debug.LogError($"Error with settings");

					return default;
				}

				return new KeyValuePair<Type, ScriptableMonoObject>(((IManager)s).GetAssociatedType(), s);
			}).Where(s => s.Key != null));
	}
}


public interface IManager
{
	Type GetAssociatedType();
}
[Serializable, HideLabel, InlineEditor]
public abstract class Manager<T> : SingletonScriptableObject<T>, IManager where T : Manager<T>
{
	public virtual Type GetAssociatedType() => null;
	// public static T Get<T>() where T : Manager => Get(typeof(T), false) as T;
	//
	// public static Manager Get(Type type, bool targetClassType)
	// {
	// 	TryPopulateDic();
	//
	// 	if (GetBox(type, targetClassType) is SettingsScriptable existingBox) return existingBox.manager;
	//
	// 	Debug.LogError($"No settings of type {type} found");
	//
	// 	return null;
	// }
	//
	//
	// public object Select(Func<object, object> func) { throw new NotImplementedException(); }
}