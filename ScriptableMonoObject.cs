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
using Sirenix.OdinInspector;


public class ScriptableMonoObject : ScriptableObject, ICacheable, ISerializationCallbackReceiver
{
	[ShowInInlineEditors]
	public new string name;
	private bool isSingleton => GetType().IsSubclassOfRawGeneric(typeof(SingletonScriptableObject<>));
	[HideIf("isSingleton")]
	public bool enabled = true;
	private static readonly string[] systemFileWords =
		{ "Singleton", "Scriptable Object", "Scriptable", "Manager" };

	protected virtual bool autoMovable => true;

	public virtual IEnumerable<string> GetCustomPathSuffixes() => default;
	public static IEnumerable<string> GetTypeDirectory(Type type, Type baseType)
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

		if (!baseTypes.Any()) return new[] { "Resources" };

		return
			baseTypes.Select(t => t.GetNiceName()
					.Split('<')[0]
					.NormalizeCamel())
				.Select(s => systemFileWords.Aggregate(s, (current, word) => current.Replace(word, ""))
					.Trim()
					.Pluralize(inputIsKnownToBeSingular: false))
				.Where(s => !string.IsNullOrEmpty(s))
				.Skip(1)
				.Prepend("Resources");
	}
	public string GetDefaultPath()
	{
		IEnumerable<string> breadcrumbs = new[] { "Assets" }
			.Concat(GetTypeDirectory(GetType(), typeof(ScriptableMonoObject))
				.Concat(GetCustomPathSuffixes())).Concat(!enabled ? new[] { "Disabled" } : new string[0]);

		return breadcrumbs.Join("/");
	}

	public static T CreateNew<T>(string name = null) where T : ScriptableMonoObject =>
		CreateNew(typeof(T), name) as T;

	[MenuItem("Tools/Scriptable Objects/Reset Locations", priority = -99998)]
	public static void MoveAllToDefaultLocation()
	{
		IEnumerable<(string, string)> oldNewPath = ScriptablesDatabase.Get(typeof(ScriptableMonoObject))
			.Where(s => s.autoMovable)
			.Select(m => (m, AssetDatabase.GetAssetPath(m)))
			.Select(ma => (ma.Item2, ma.Item1.GetDefaultPath() + $"/{ma.Item1.name}.asset"))
			.Where(t => t.Item1 != t.Item2);

		if (!oldNewPath.Any())
		{
			EditorUtility.DisplayDialog("Move Scriptable Objects", "No scriptable objects to move", "OK");
			return;
		}

		// Wait for user confirmation
		string directoryChanges = string.Join("\n", oldNewPath.Select(t => $"{t.Item1} -> {t.Item2}"));
		Debug.Log($"{oldNewPath.Count()} changes:\n{directoryChanges}");
		if (!EditorUtility.DisplayDialog("Move Scriptable Objects",
			    $"Move {oldNewPath.Count()} scriptable objects?\n{directoryChanges}",
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
		return SaveExisting(newSingleton);
	}
	public enum OverwriteMode
	{
		Overwrite, Increment, Ignore
	}
	public static ScriptableMonoObject SaveExisting(ScriptableMonoObject existingObject, string name = null,
		OverwriteMode overwriteMode = OverwriteMode.Increment)
	{
		string path = existingObject.GetDefaultPath();
		string typeName = existingObject.GetType().Name.Humanize(LetterCasing.Title);
		name ??= !existingObject.name.IsNullOrWhiteSpace()
			? existingObject.name
			: typeName;
		string assetPath = $"{path}/{name}.asset";
		path.EnsureFolderExists();

		OverwriteMode? overwrite = null;
		if (overwriteMode is OverwriteMode.Increment)
		{
			path = path.GetIncrementalFolderNumber();
			overwrite = OverwriteMode.Increment;
		}
		else
		{
			overwrite = !File.Exists(assetPath) ? null : overwriteMode;
		}

		ScriptableMonoObject createdAsset = null;
		if (overwrite is not OverwriteMode.Ignore)
		{
			AssetDatabase.CreateAsset(existingObject, assetPath);
			AssetDatabase.SaveAssets();
			createdAsset = AssetDatabase.LoadAssetAtPath<ScriptableMonoObject>(assetPath);
		}
		Debug.Log(
			string.Format("{0} {1} at {2}",
				overwrite switch
				{
					OverwriteMode.Ignore => "Skipped existing",
					OverwriteMode.Overwrite => "Overwrote",
					_ => "Created new"
				},
				typeName,
				assetPath),
			createdAsset);
		return createdAsset;
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
	protected virtual void Update() {}
	protected virtual void Deinitialize() {}

	protected static Coroutine StartCoroutine(IEnumerator coro, bool allowInEditor = false) =>
		ScriptHelper.StartCoroutine(coro, allowInEditor);
	protected void StopCoroutine(Coroutine coro) => ScriptHelper.StopCoroutine(coro);

}