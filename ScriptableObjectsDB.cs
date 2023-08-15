using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(-9999)]
public class ScriptableObjectsDB : SingletonScriptableObject<ScriptableObjectsDB>
{
	public string[] paths;


#if UNITY_EDITOR
	[Button]
	public void SetPaths() => _i.paths = Resources.LoadAll<ScriptableObject>("")
		.Select(x => AssetDatabase.GetAssetPath(x)
			.Replace("Assets/Resources/", "")
			.Replace(".asset", ""))
		.ToArray();
#endif
}

public static class ScriptableObjectsDBExtensions
{
	// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Button]
	public static void LoadAssets()
	{
		Debug.Log($"Starting loading assets");
		var db = Resources.Load<ScriptableObjectsDB>("Scriptable Objects DB");

		foreach (string path in db.paths)
		{
			Debug.Log($"Loading '{path}'");
			try
			{
				Resources.Load(path);
			}
			catch (Exception e)
			{
				Debug.LogError($"Error loading '{path}' : {e.Message}");
			}
		}
	}
}