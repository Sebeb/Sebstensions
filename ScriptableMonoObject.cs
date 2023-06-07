using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
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
			TryUpdateMonoObjects();
			return _monoScripts;
		}
	}

	private static bool TryUpdateMonoObjects()
	{
		//Update monoscripts listing if not previously updated or if we're in the editor
		if (Application.isEditor || _monoScripts == null || _monoScripts.Length == 0)
		{
			_monoScripts = Resources.LoadAll<ScriptableMonoObject>("");
			_monoScriptsByType = Resources.LoadAll<ScriptableMonoObject>("")
				.SelectMany(m => m.GetType().GetBaseTypes().PrependWith(m.GetType())
					.Select(t => new Tuple <Type, ScriptableMonoObject>(t, m)))
				.Where(t => t != null)
				.GroupBy(t => t.Item1, t => t.Item2)
				.ToDictionary(g => g.Key, g => g.ToArray());
			return true;
		}

		return false;
	}

	public static void StartMonoScripts()
	{
		if (!Application.isPlaying) return;

		foreach (ScriptableMonoObject monoScript in monoScripts)
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
		foreach (ScriptableMonoObject monoScript in monoScripts)
		{
			monoScript.ScriptReset();
		}
	}


	public virtual void ScriptAwake() {}
	public virtual void ScriptReset() {}

	public static IEnumerable<T> GetAllScriptables<T>()
	{
		TryUpdateMonoObjects();
		return monoScripts.Length == 0 ? null : _monoScriptsByType[typeof(T)].Cast<T>();
	}


	public static T GetScriptableSingleton<T>() where T : ScriptableMonoObject =>
		(T)monoScripts.FirstOrDefault(s => s.GetType() == typeof(T));
}