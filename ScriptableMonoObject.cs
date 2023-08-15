using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Humanizer;


public class ScriptableMonoObject : ScriptableObject, ICacheable, ISerializationCallbackReceiver
{
	[HideInInspector]
	public new string name;
	private static readonly string[] systemFileWords = { "Singleton", "Scriptable Object", "Scriptable", "Manager" };

	public static string TypeToDirectory(Type type, Type baseType)
	{
		// ScriptablesDatabase.Refresh();
		IEnumerable<Type> baseTypes = type.GetBaseTypes(true);

		baseTypes = baseTypes.Where(t => !t.IsInterface);
		baseTypes = baseTypes.Reverse();
		baseTypes = baseTypes.SkipWhile(t => t != baseType);
		// if (ScriptablesDatabase.Get(type).Count() == 1)
		// {
		// 	baseTypes = baseTypes.SkipLast(1);
		// }

		if (!baseTypes.Any()) return "";

		var path = string.Join("/",
			baseTypes.Select(t => t.GetNiceName()
					.Split('<')[0]
					.NormalizeCamel())
				.Select(s => systemFileWords.Aggregate(s, (current, word) => current.Replace(word, ""))
					.Trim()
					.Pluralize(inputIsKnownToBeSingular: false))
				.Where(s => !string.IsNullOrEmpty(s))
				.Skip(1)
				.Prepend("Data"));
		path += "/";
		return path;
	}

	public static T CreateNew<T>(string name = null) where T : ScriptableMonoObject => CreateNew(typeof(T), name) as T;

	[MenuItem("Tools/Scriptable Objects/Reset Locations", priority = -99998)]
	public static void MoveAllToDefaultLocation()
	{
		IEnumerable<(string, string)> oldNewPath = ScriptablesDatabase.Get(typeof(ScriptableMonoObject))
			.Select(m => (m, AssetDatabase.GetAssetPath(m)))
			.Select(ma => (ma.Item2, $"Assets/{TypeToDirectory(ma.Item1.GetType(), typeof(ScriptableMonoObject))}{ma.Item1.name}.asset"))
			.Where(t => t.Item1 != t.Item2);

		// Wait for user confirmaion
		if (!EditorUtility.DisplayDialog("Move Scriptable Objects",
			    $"Move {oldNewPath.Count()} scriptable objects?\n"
			    + string.Join("\n", oldNewPath.Select(t => $"{t.Item1} -> {t.Item2}")),
			    "Yes",
			    "No")) return;

		foreach ((string oldPath, string newPath) in oldNewPath)
		{
			if (!Path.GetDirectoryName(newPath).EnsureFolderExists()) AssetDatabase.Refresh();
			AssetDatabase.MoveAsset(oldPath, newPath);

			Debug.Log($"Moving {oldPath} to {newPath}");
		}

		AssetDatabase.SaveAssets();

	}

	public static ScriptableMonoObject CreateNew(Type type, string name = null)
	{
		if (!(CreateInstance(type) is ScriptableMonoObject newSingleton))
		{
			Debug.LogError("Could not create new singleton of type " + type.GetNiceFullName());
			return null;
		}

		string path = $"Assets/{TypeToDirectory(type, typeof(ScriptableMonoObject))}";
		name ??= type.Name.NormalizeCamel();
		string assetPath = path + name + ".asset";
		path.EnsureFolderExists();

		AssetDatabase.CreateAsset(newSingleton, assetPath);
		AssetDatabase.SaveAssets();
		Debug.Log($"Created new {type.Name.NormalizeCamel()} at {assetPath}");
		return AssetDatabase.LoadAssetAtPath<ScriptableMonoObject>(assetPath);
	}

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

	public Coroutine StartCoroutine(IEnumerator coroutine) => ScriptHelper.StartCoroutine(coroutine);

	public virtual void OnBeforeSerialize() => SetAssetName();

	public void OnAfterDeserialize() {}

	#region Database

	#endregion

	public static void StartMonoScripts()
	{
		if (!Application.isPlaying) return;

		foreach (ScriptableMonoObject monoScript in ScriptablesDatabase.Get<ScriptableMonoObject>())
		{
			monoScript.ScriptAwake();
		}
	}

	//TODO Init all singletons which implement SingletonScriptableObject<>
	private static bool initedOnce;
	public static void InitSingletons()
	{
		foreach (ScriptableMonoObject monoScript in ScriptablesDatabase.Get<ScriptableMonoObject>())
		{
			if (initedOnce && !monoScript.ReInitOnRestart()) { continue; }

			monoScript.Initialize();
		}
		initedOnce = true;
	}
	
	public static void UpdateMonoScripts()
	{
		if (!Application.isPlaying) { return; }
		foreach (ScriptableMonoObject monoScript in ScriptablesDatabase.Get<ScriptableMonoObject>())
		{
			monoScript.Update();
		}
	}

	public virtual bool ReInitOnRestart() => true;

	public static void ResetMonoScripts(bool isRestart)
	{
		foreach (ScriptableMonoObject monoScript in ScriptablesDatabase.Get(typeof(SingletonScriptableObject<>)))
		{
			if (isRestart && !monoScript.ReInitOnRestart()) { continue; }

			monoScript.Deinitialize();
		}
	}

	protected virtual void Initialize() {}
	protected virtual void ScriptAwake() {}
	protected virtual void Update(){}
	protected virtual void Deinitialize() {}
	
	protected static Coroutine StartCoroutine(IEnumerator coro, bool allowInEditor = false) =>
		ScriptHelper.StartCoroutine(coro, allowInEditor);
	protected void StopCoroutine(Coroutine coro) => ScriptHelper.StopCoroutine(coro);

}