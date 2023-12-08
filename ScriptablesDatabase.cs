using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using FlyingWormConsole3.FullSerializer.Internal;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
#endif
using UnityEngine;


public class ScriptablesDatabase : SerializedScriptableObject
#if UNITY_EDITOR
	,
	IPreprocessBuild
#endif
{
	private static ScriptablesDatabase __i;
	internal static ScriptablesDatabase _i
	{
		get
		{
			if (__i == null)
			{
				__i = Resources.Load<ScriptablesDatabase>("ScriptablesCache");
			}

			if (__i == null)
			{
			#if UNITY_EDITOR
				__i = CreateInstance<ScriptablesDatabase>();
				AssetDatabase.CreateAsset(__i, "Assets/Resources/ScriptablesCache.asset");
				AssetDatabase.SaveAssets();
			#else
				Debug.LogError("No ScriptablesCache asset found");
			#endif
			}

			return __i;
		}
	}

#if UNITY_EDITOR
	[DidReloadScripts, InitializeOnLoadMethod,
 #else
[
 #endif
	 RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void SetSingleton() => __i ??= _i;

	public string[][] scriptables2D;
	[SerializeField, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout)]
	private SDictionary<string, int> scriptablesIByType;
	private Dictionary<Type, ScriptableMonoObject[]> typeScriptablesDic = new();


	public static bool dirty = false;
	[ExecuteOnReload, RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded),
 #if UNITY_EDITOR
	 InitializeOnLoadMethod]
#else
	]
#endif
	public static bool TryRefresh()
	{
	#if !UNITY_EDITOR
		return false;
		}
	#else
		//Update monoscripts listing if not previously updated, or if we are in editor and not playing (max once per frame)
		if (!dirty) return false;

		Refresh();
		return true;
	}


	public int callbackOrder { get; }
	public void OnPreprocessBuild(BuildTarget target, string path) => Refresh();

	[MenuItem("Tools/Scriptable Objects/Refresh Database #%s")]
	private static void MenuRefresh() => Refresh();

	public static Action OnRefresh;
	[Button]
	public static void Refresh()
	{
		Dictionary<Type, ScriptableMonoObject[]> oldDb =
			new Dictionary<Type, ScriptableMonoObject[]>(_i.typeScriptablesDic);
		Debug.Log(AppDomain.CurrentDomain.GetAssemblies()
			.Select(t => t.GetName().Name).Join());

		IEnumerable<Type> cachableType = AppDomain.CurrentDomain.GetAssemblies()
			.Where(a => a.GetName().Name == "PESSystems" || a.GetName().Name == "Seb")
			.SelectMany(a => a.GetTypes())
			.Where(t => t.CalculateInheritanceDistance(typeof(ICacheable)) >= 0);

		cachableType =
			cachableType.Concat(Reflection.GetAllSingletonScriptChildrenTypes<ScriptableMonoObject>())
				.Distinct();

		_i.scriptablesIByType.Clear();
		Dictionary<Type, List<ScriptableMonoObject>> newTypeScriptablesDic = new();
		foreach (IEnumerable<ScriptableMonoObject> instances in cachableType
			         .Select(t =>
				         //Hacky fix to bypass Unity selecting some odd unrelated assets
				         AssetDatabase.FindAssets("t:" + t,
						         new[] { "Assets/Resources", "Assets/Resources (Editor)", "Assets/Game Systems/" })
					         .Select(g => AssetDatabase.GUIDToAssetPath(g))
					         .Select(p =>
						         AssetDatabase.LoadAssetAtPath<ScriptableMonoObject>(p))
					         .Where(s => s != null && s.enabled)))
		{
			foreach (ScriptableMonoObject instance in instances)
			{
				foreach (Type parentType in cachableType.Where(t =>
					         instance.GetType().CalculateInheritanceDistance(t) >= 0))
				{
					newTypeScriptablesDic.AddToCollection(parentType, instance, uniqueOnly: true);
				}
			}
		}
		_i.typeScriptablesDic = new Dictionary<Type, ScriptableMonoObject[]>(newTypeScriptablesDic
			.Select(kvp => new KeyValuePair<Type, ScriptableMonoObject[]>(kvp.Key, kvp.Value.ToArray())));

		_i.scriptables2D = new string[_i.typeScriptablesDic.Keys.Count()][];

		foreach (var row in _i.typeScriptablesDic.Select((k, i) => new { k, i }))
		{
			_i.scriptables2D[row.i] = row.k.Value.Select(s =>
				{
					string path = AssetDatabase.GetAssetPath(s);
					if (path[..17] != "Assets/Resources/")
					{
						return path;
					}
					else
					{
						path = path.Replace("Assets/Resources/", "")
							.Replace(".asset", "");
					}

					return path;
				})
				.ToArray();
			_i.scriptablesIByType[row.k.Key.Name] = row.i;
		}
		EditorUtility.SetDirty(_i);
		AssetDatabase.SaveAssetIfDirty(_i);

		dirty = false;

		Dictionary<Type, int> changeCount = _i.typeScriptablesDic.Keys.Union(oldDb.Keys)
			.ToDictionary(t => t, t =>
				(_i.typeScriptablesDic.TryGetValue(t, out ScriptableMonoObject[] newSmos) ? newSmos.Length : 0)
				- (oldDb.TryGetValue(t, out ScriptableMonoObject[] oldSmos) ? oldSmos.Length : 0));

		Debug.Log($"Rebuilt scriptables cache. {changeCount.Values.Count(v => v != 0)} types changed sizes.\n"
			+ changeCount.Select(kvp =>
					$"{kvp.Key}: {(_i.typeScriptablesDic.TryGetValue(kvp.Key, out ScriptableMonoObject[] newSmos) ? newSmos.Length : 0)} {(kvp.Value != 0 ? $"({(kvp.Value > 0 ? "+" : "-")}{kvp.Value})" : "")}")
				.Join("\n"));

		OnRefresh?.Invoke();
	}
#endif


	public static IEnumerable<T> Get<T>()
	{
		IEnumerable<T> collection = null;
		IEnumerable<ScriptableMonoObject> scriptableMonoObjects = Get(typeof(T));
		try
		{
			collection = scriptableMonoObjects.Cast<T>();
		}
		catch (InvalidCastException e)
		{
			Debug.LogError(
				$"Error casting the following types to {typeof(T)}: {scriptableMonoObjects.Where(s => (ScriptableMonoObject)s == null).Select(s => s.GetType().ToString()).Join()}");
			throw;
		}
		return collection;

	}

	public static IEnumerable<ScriptableMonoObject> Get(Type type, bool silent = false)
	{
		TryRefresh();
		if (_i.typeScriptablesDic.TryGetValue(type, out ScriptableMonoObject[] scriptables))
		{
			return scriptables;
		}

		if (_i.scriptablesIByType.TryGetValue(type.Name, out int index))
		{
			_i.typeScriptablesDic[type] = _i.scriptables2D[index]
				.Select(p =>
				{
					var smo = Resources.Load(p) as ScriptableMonoObject;
				#if UNITY_EDITOR
					if (smo == null) smo = AssetDatabase.LoadAssetAtPath(p, type) as ScriptableMonoObject;
				#endif
					if (smo == null && Debug.isDebugBuild && p.Length > 17 && p[..17] == "Assets/Resources/")
						Debug.LogError($"Error loading {type.Name} at {p}");
					return smo;
				})
			#if UNITY_EDITOR
				.Where(s => s != null && s.GetType().CalculateInheritanceDistance(type) >= 0)
			#else
				.WhereNotNull()
			#endif
				.ToArray();
			if (_i.scriptables2D[index].Length != _i.typeScriptablesDic[type].Length && !dirty)
			{
				dirty = true;
				Debug.Log($"Set Scriptables Database to dirty due to null resources loading for {type}");
			}
			return _i.typeScriptablesDic[type];
		}

		if (!silent) Debug.LogError($"No scriptable objects of type {type} found");
		return Enumerable.Empty<ScriptableMonoObject>();
	}
}

#if UNITY_EDITOR
class ScriptablesDatabaseEditorCallbacks : UnityEditor.AssetModificationProcessor
{
	private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
	{
		SetDirty(assetPath);
		return AssetDeleteResult.DidNotDelete;
	}
	private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
	{
		SetDirty(sourcePath, destinationPath);
		return AssetMoveResult.DidNotMove;
	}
	private static void OnWillCreateAsset(string assetName) => SetDirty(assetName);
	private static string[] OnWillSaveAssets(string[] paths)
	{
		if (paths.Any(p => p.Contains("Resources/")))
		{
			ScriptablesDatabase.TryRefresh();
		}
		return paths;
	}
	private static void SetDirty(params string[] paths)
	{
		if (ScriptablesDatabase.dirty || !paths.Any(p => p.Contains("Resources/"))) return;

		Debug.Log($"Set Scriptables Database to dirty due to changes with {paths.Join()}");
		ScriptablesDatabase.dirty = true;
	}
}
#endif