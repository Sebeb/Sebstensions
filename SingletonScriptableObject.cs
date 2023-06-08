﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public interface ICacheable {}

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on the editor, or null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>
public abstract class SingletonScriptableObject<T> : ScriptableMonoObject, ICacheable
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
				Debug.LogError("Multiple "
					+ _i.GetType()
					+ " detected. Using "
					+ AssetDatabase.GetAssetPath(instances[0])
					+ ". Consider destroying imposters.");
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

public class ScriptableSingletonHelper : MonoBehaviour
{
#if UNITY_EDITOR

	[UnityEditor.Callbacks.DidReloadScripts, MenuItem("Tools/Scriptable Objects/Refresh Singletons")]
	public static void DebugSingletons()
	{
		bool assetsMade = false;
		IEnumerable<Type> singletons =
			Reflection.GetAllSingletonScriptChildrenTypes<ScriptableMonoObject>();

		foreach (Type monoObjectType in singletons)
		{
			string[] guids = AssetDatabase.FindAssets("t:" + monoObjectType);
			string name = monoObjectType.ToString().NormalizeCamel();
			if (guids.Length > 1)
			{
				Debug.Log(
					$"Multiple instances of {monoObjectType} found at:{string.Join("\n", guids.Select(AssetDatabase.GUIDToAssetPath))}");
			}
			else if (guids.Length == 0)
			{
				ScriptableMonoObject.CreateNew(monoObjectType);
			}
		}

		if (assetsMade)
		{
			AssetDatabase.SaveAssets();
		}
	}
#endif
}