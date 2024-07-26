#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class DebugData : SingletonScriptableObject<DebugData>, IStartCallback
{
	public AccessMode accessMode = AccessMode.EditorOnly;
	public enum AccessMode { AlwaysAccessible, DebugBuildsOnly, EditorOnly, Inaccessible }
	public static bool accessible => SetInstance(true) != null && (_i.accessMode == AccessMode.AlwaysAccessible
		|| _i.accessMode == AccessMode.DebugBuildsOnly && Debug.isDebugBuild
		|| _i.accessMode == AccessMode.EditorOnly && Application.isEditor);

	public SDictionary<string, string> data = new();
	protected override bool isEditorResource => true;

	public static bool TryGetValue(string key, out string value)
	{
		if (!accessible || !_i.data.TryGetValue(key, out value))
		{
			value = null;
			return false;
		}
		return true;
	}

	public static bool TrySetValue(string key, string value)
	{
		if (!accessible) return false;
		_i.data[key] = value;
		//Set Dirty
	#if UNITY_EDITOR
		EditorUtility.SetDirty(_i);
	#endif
		return true;
	}

	public static void TryRemoveValue(string key)
	{
		if (!accessible) return;
		_i.data.Remove(key);
	#if UNITY_EDITOR
		EditorUtility.SetDirty(_i);
	#endif
	}
	public void ScriptStart()
	{
		if (accessible) { DebugCorner.AddDebugText(-99, "Debug Data Used", 5); }
	}
}