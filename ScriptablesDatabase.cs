using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
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
				__i = Resources.Load<ScriptablesDatabase>("SystemData");
			}

			if (__i == null)
			{
			#if UNITY_EDITOR
				__i = CreateInstance<ScriptablesDatabase>();
				AssetDatabase.CreateAsset(__i, "Assets/Resources/SystemData.asset");
				AssetDatabase.SaveAssets();
			#else
				Debug.LogError("No SystemData asset found");
			#endif
			}

			return __i;
		}
	}

	[DidReloadScripts, InitializeOnLoadMethod,
	 RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void SetSingleton() => __i ??= _i;

	public string[][] scriptables2D;
	[SerializeField]
	private SDictionary<string, int> scriptablesIByType;
	private Dictionary<Type, ScriptableMonoObject[]> typeScriptablesDic = new();


	private static int lastFrameRefreshed;
	public static bool TryRefresh()
	{
		//Update monoscripts listing if not previously updated, or if we are in editor and not playing (max once per frame)
		if (Time.frameCount == lastFrameRefreshed
		    || !(Application.isEditor && !Application.isPlaying)
		    && _i.scriptablesIByType != null
		    && _i.scriptablesIByType.Any())
			return false;

		Refresh();
		return true;
	}


#if UNITY_EDITOR
	public int callbackOrder { get; }
	public void OnPreprocessBuild(BuildTarget target, string path) => Refresh();
#endif

	[MenuItem("Tools/Scriptable Objects/Refresh Database")]
	private static void MenuRefresh() => Refresh(false);


	[ExecuteOnReload, RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded),
	 InitializeOnLoadMethod]
	private static void AutoRefresh() => Refresh(true);

	[Button]
	public static void Refresh(bool silent = true)
	{
		IEnumerable<Type> cachableType = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(t => t.ImplementsOrInherits(typeof(ICacheable))
				&& t.GetInheritanceDistance(typeof(ICacheable)) > 0);

		cachableType =
			cachableType.Concat(Reflection.GetAllSingletonScriptChildrenTypes<ScriptableMonoObject>())
				.Distinct();

		_i.scriptablesIByType.Clear();
		_i.typeScriptablesDic = cachableType
			.Select(t => new KeyValuePair<Type, ScriptableMonoObject[]>(t,
				AssetDatabase.FindAssets("t:" + t)
					.Select(g =>
						AssetDatabase.LoadAssetAtPath<ScriptableMonoObject>(AssetDatabase.GUIDToAssetPath(g)))
					.Where(s => s.enabled)
					.ToArray())).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		_i.scriptables2D = new string[_i.typeScriptablesDic.Keys.Count()][];

		foreach (var row in _i.typeScriptablesDic.Select((k, i) => new { k, i }))
		{
			_i.scriptables2D[row.i] = row.k.Value.Select(s => AssetDatabase.GetAssetPath(s)
				.Replace("Assets/Resources/", "")).ToArray();
			_i.scriptablesIByType[row.k.Key.Name] = row.i;
		}

		lastFrameRefreshed = Time.frameCount;

		if (silent) return;

		Debug.Log(Enumerable.Select(_i.typeScriptablesDic, kvp => $"{kvp.Key} ({kvp.Value.Length})").Join());
	}


	public static IEnumerable<T> Get<T>() =>
		Get(typeof(T)).Cast<T>();

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
				.Select(p => Resources.Load(p) as ScriptableMonoObject)
				.ToArray();
			return _i.typeScriptablesDic[type];
		}

		if (!silent) Debug.LogError($"No scriptable objects of type {type} found");
		return Enumerable.Empty<ScriptableMonoObject>();
	}
}