using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


public class ScriptablesDatabase : SerializedScriptableObject
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

	public ScriptableMonoObject[][] scriptables2D;
	[SerializeField]
	private SDictionary<string, int> scriptablesIByType;
	private Dictionary<Type, ScriptableMonoObject[]> typeScriptablesDic = new();


	public static bool TryRefresh()
	{
		//Update monoscripts listing if not previously updated or if we're in the editor
		if (!Application.isEditor && _i.typeScriptablesDic != null && _i.typeScriptablesDic.Any()) return false;

		Refresh();
		return true;

	}


	[MenuItem("Tools/Scriptable Objects/Refresh Database")]
	private static void _Refresh() => Refresh(false);

	[Button]
	public static void ManualRefresh() => Refresh(true);

	[ExecuteOnReload, RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void Refresh() => Refresh(true);

	public static void Refresh(bool silent = true)
	{
		IEnumerable<Type> cachableType = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(t => t.ImplementsOrInherits(typeof(ICacheable))
				&& t.GetInheritanceDistance(typeof(ICacheable)) > 0);

		cachableType =
			cachableType.Concat<Type>(Reflection.GetAllSingletonScriptChildrenTypes<ScriptableMonoObject>()).Distinct();

		_i.scriptablesIByType.Clear();
		_i.typeScriptablesDic = cachableType
			.Select(t => new KeyValuePair<Type, ScriptableMonoObject[]>(t,
				AssetDatabase.FindAssets("t:" + t)
					.Select(g =>
						AssetDatabase.LoadAssetAtPath<ScriptableMonoObject>(AssetDatabase.GUIDToAssetPath(g)))
					.ToArray())).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

		_i.scriptables2D = new ScriptableMonoObject[_i.typeScriptablesDic.Keys.Count()][];

		foreach (var row in _i.typeScriptablesDic.Select((k, i) => new { k, i }))
		{
			_i.scriptables2D[row.i] = row.k.Value;
			_i.scriptablesIByType[row.k.Key.Name] = row.i;
		}

		if (silent) return;

		Debug.Log(string.Join("\n",
			Enumerable.Select(_i.typeScriptablesDic,
				kvp =>
					$"({kvp.Value.Length}) {kvp.Key}:  {string.Join(", ", Enumerable.Select<ScriptableMonoObject, string>(kvp.Value, s => s.name))}")));
	}


	public static IEnumerable<T> Get<T>() =>
		Get(typeof(T)).Cast<T>();

	public static IEnumerable<ScriptableMonoObject> Get(Type type)
	{
		TryRefresh();
		if (_i.typeScriptablesDic.TryGetValue(type, out ScriptableMonoObject[] scriptables))
		{
			return scriptables;
		}

		if (_i.scriptablesIByType.TryGetValue(type.Name, out int index))
		{
			_i.typeScriptablesDic[type] = _i.scriptables2D[index];
			return _i.scriptables2D[index];
		}

		// Debug.LogError($"No scriptable objects of type {type} found");
		return Enumerable.Empty<ScriptableMonoObject>();
	}
}