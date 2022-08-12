using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;


public abstract class SingletonMonoBehaviour<T> : CustomMono where T : SingletonMonoBehaviour<T>
{


	protected override void Assign() => SetInstance();
	private void SetInstance()
	{

		if (_i != null)
		{
			if (_i == this) { return; }

			Debug.LogError($"Multiple instances of{GetType()} found", this);
			return;
		}
		// Debug.Log($"Set instance {GetType()}", this);

		_i = this as T;
	}
	// ReSharper disable once InconsistentNaming
	public static T _i
	{
		get;
		private set;
	}

}

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on the editor, or null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>
public abstract class SingletonScriptableObject<T> : ScriptableMonoObject
	where T : ScriptableMonoObject
{
	static T _instance = null;


	protected static T SetInstance()
	{
		// if (_instance != null) { return _instance; }

		T[] instances = Resources.LoadAll<T>("")
			#if UNITY_EDITOR
				.Where(i => AssetDatabase.GetAssetPath(i) != null).ToArray()
		#endif
			;
		if (instances.Length == 0)
		{
		#if UNITY_EDITOR
			_instance = ScriptableObject.CreateInstance<T>();
			AssetDatabase.CreateAsset(_instance,
				"Assets/Resources/" + typeof(T).ToString().NormalizeCamel() + ".asset");
			AssetDatabase.SaveAssets();
			Debug.Log("Created new settings file for " + typeof(T).ToString().NormalizeCamel());
		#else
            Debug.LogError("No scriptable object singleton of type " + typeof(T));
		#endif
		}
		else
		{
			if (instances.Length > 1)
			{
			#if UNITY_EDITOR
				Debug.LogError("Multiple " + _i.GetType() + " detected. Using " +
					AssetDatabase.GetAssetPath(instances[0]) + ". Consider destorying imposters.");
			#endif
			}

			_instance = instances[0];
		}
		return _instance;
	}
	// ReSharper disable once InconsistentNaming
	public static T _i
	{
		get
		{
			if (_instance == null)
			{
				SetInstance();
			}
			return _instance;
		}
	}

}

public abstract class ScriptableMonoObject : ScriptableObject, ISerializationCallbackReceiver
{
	[ReadOnly]
	public new string name;
	private static ScriptableMonoObject[] _monoScripts;
	protected void SetAssetName(string newName = null)
	{
	#if UNITY_EDITOR
		string assetPath = AssetDatabase.GetAssetPath(this);
		if (assetPath.IsNullOrEmpty()) { return; }
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
			}
			return _monoScripts;
		}
		set => _monoScripts = value;
	}

	public static void StartMonoScripts()
	{
		if (!Application.isPlaying) { return; }
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

	public static T GetScriptableSingleton<T>() where T : ScriptableMonoObject =>
		(T)monoScripts.FirstOrDefault(s => s.GetType() == typeof(T));
}