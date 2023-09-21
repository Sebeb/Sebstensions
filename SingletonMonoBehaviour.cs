using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class SingletonMonoBehaviour<T> : CustomMono where T : SingletonMonoBehaviour<T>
{
	protected bool AssertSingleton(bool thisInstance, bool destroyOthers = true)
	{
		List<SingletonMonoBehaviour<T>> instances =
			new List<SingletonMonoBehaviour<T>>(FindObjectsOfType<T>()
				.Select(o => o as SingletonMonoBehaviour<T>));
		instances.Remove(this);

		if (instances.Count == 0)
		{
			_instance = this as T;
			return true;
		}
		else
		{
			if (thisInstance)
			{
				_instance = this as T;
				foreach (SingletonMonoBehaviour<T> instance in instances)
				{
					instance.gameObject.Destroy();
				}
				return true;
			}
			else
			{
				gameObject.Destroy();
				return false;
			}
		}
	}
	protected static T GetInstance(bool quiet = false)
	{
		if (_instance) { return _instance; }

		var instances = new List<T>(FindObjectsOfType<T>());
		if (instances.Count == 0)
		{
			if (ScriptHelper.quitting)
			{
				if (!quiet) Debug.LogError($"{typeof(T)} singleton is no longer accessible since the application is quitting");
				return null;
			}
			if (!Application.isPlaying)
			{
				if (!quiet) Debug.LogError($"{typeof(T)} singleton only exists in playtime");
				return null;
			}
			_instance = MonoTools.MakeSingleton<T>();
		}
		else
		{
			if (instances.Count > 1)
			{
				Debug.LogError($"Multiple {typeof(T)} detected. ");
			}

			_instance = instances[0];
		}
		return _instance;
	}

	public static T _i => _instance ??= GetInstance(true);
	private static T _instance;
}

public static class MonoTools
{
	public static Transform singletonsParent;

	public static T MakeSingleton<T>() where T : Component
	{
		if (singletonsParent == null)
		{
			singletonsParent = ((Transform)null).GetOrMakeChild("Singletons");
			// Debug.Log("Made new singletons parent!");
			Object.DontDestroyOnLoad(singletonsParent.gameObject);
			singletonsParent.transform.SetSiblingIndex(0);
		}
		T newInstance = new GameObject(typeof(T).ToString()).AddComponent<T>();
		newInstance.transform.SetParent(singletonsParent.transform);

		return newInstance;
	}
}