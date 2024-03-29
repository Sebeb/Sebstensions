﻿using System;
using System.Collections.Generic;
using System.Linq;
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


	protected static T SetInstance(bool silent = false)
	{
		if (_instance != null) return _instance;

		IEnumerable<T> instances = ScriptablesDatabase.Get<T>();

		if (!instances.Any())
		{
		#if UNITY_EDITOR
			_instance = CreateInstance<T>();
			AssetDatabase.CreateAsset(_instance,
				"Assets/Resources/" + typeof(T).ToString().NormalizeCamel() + ".asset");
			AssetDatabase.SaveAssets();
			Debug.Log("Created new settings file for " + typeof(T).ToString().NormalizeCamel());
		#else
           if (!silent) Debug.LogError("No scriptable object singleton of type " + typeof(T));
		#endif
		}
		else
		{
			if (instances.Count() > 1)
			{
			#if UNITY_EDITOR
				Debug.LogError("Multiple "
					+ instances.First().GetType()
					+ " detected. Using "
					+ AssetDatabase.GetAssetPath(instances.FirstOrDefault())
					+ ". Consider destroying imposters.");
			#endif
			}

			_instance = instances.FirstOrDefault();
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

public class ScriptableSingletonHelper
{
#if UNITY_EDITOR
	ScriptableSingletonHelper()
	{
		ScriptablesDatabase.OnRefresh += DebugSingletons;
	}
	
	[MenuItem("Tools/Scriptable Objects/Refresh Singletons", priority = -99998)]
	public static void DebugSingletons()
	{
		bool assetsMade = false;
		IEnumerable<Type> singletons = Reflection.GetAllSingletonScriptChildrenTypes<ScriptableMonoObject>();

		foreach (Type monoObjectType in singletons)
		{
			IEnumerable<ScriptableMonoObject> scriptableMonoObjects =
				ScriptablesDatabase.Get(monoObjectType).ToList();
			if (scriptableMonoObjects.Count() > 1)
			{
				Debug.Log(
					$"Multiple instances of {monoObjectType} found at: {string.Join(", ", scriptableMonoObjects.Select(s => AssetDatabase.GetAssetPath(s)))}");
			}
			else if (scriptableMonoObjects.Count() == 0)
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