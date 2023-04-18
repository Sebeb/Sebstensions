using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public abstract class ScriptableMonoObject : ScriptableObject, ISerializationCallbackReceiver
{
	[ReadOnly]
	public new string name;
	private static ScriptableMonoObject[] _monoScripts;
	private static Dictionary<Type, ScriptableMonoObject[]> _monoScriptsByType;

	protected void SetAssetName(string newName = null)
	{
	#if UNITY_EDITOR
		string assetPath = AssetDatabase.GetAssetPath(this);
		if (assetPath.IsNullOrEmpty())
		{
			return;
		}

		string assetName = assetPath.Split('/').Last().Split('.').First();

		if (!newName.IsNullOrEmpty())
		{
			name = newName;
			if (newName != assetName)
			{
				AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), newName);
			}
		}
		else
		{
			name = assetName;
		}

	#endif
	}

	public virtual void OnBeforeSerialize() => SetAssetName();

	public void OnAfterDeserialize()
	{
	}

	public static ScriptableMonoObject[] monoScripts
	{
		get
		{
			//Update monoscripts listing if not previously updated or if we're in the editor
			if (Application.isEditor || _monoScripts == null || _monoScripts.Length == 0)
			{
				_monoScripts = Resources.LoadAll<ScriptableMonoObject>("");
				_monoScriptsByType = Resources.LoadAll<ScriptableMonoObject>("")
					.GroupBy(m => m.GetType())
					.ToDictionary(g => g.Key, g => g.ToArray());
			}

			return _monoScripts;
		}
		set => _monoScripts = value;
	}

	public static void StartMonoScripts()
	{
		if (!Application.isPlaying)
		{
			return;
		}

		foreach (ScriptableMonoObject monoScript in ScriptableMonoObject.monoScripts)
		{
			monoScript.ScriptAwake();
		}
	}

	//TODO Init all singletons which implement SingletonScriptableObject<>
	public static void InitSingletons()
	{
		//Select all types which inherit the generic class SingletonScriptableObject
		IEnumerable<Type> singletonTypes = from t in Assembly.GetExecutingAssembly().GetTypes()
			where t.IsClass && !t.IsAbstract && t.GetInheritanceHierarchy().Any(t =>
				t.IsGenericType &&
				t.GetGenericTypeDefinition() == typeof(SingletonScriptableObject<>))
			select t;

		Debug.Log(string.Join(", ", singletonTypes.Select(t => t.ToString())));
	}

	public static void ResetMonoScripts()
	{
		foreach (ScriptableMonoObject monoScript in ScriptableMonoObject.monoScripts)
		{
			monoScript.ScriptReset();
		}
	}


	public virtual void ScriptAwake() {}
	public virtual void ScriptReset() {}

	public static IEnumerable<T> GetAllScriptables<T>() where T : ScriptableMonoObject
		=>
			monoScripts.Length == 0 ? null : _monoScriptsByType[typeof(T)].Cast<T>();


	public static T GetScriptableSingleton<T>() where T : ScriptableMonoObject =>
		(T)monoScripts.FirstOrDefault(s => s.GetType() == typeof(T));
}