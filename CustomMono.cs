using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using Weaver;

public abstract class CustomMono : MonoBehaviour
{
	public static PlayModeStateChange playmodeState;
	public static Action<Vector2> OnScreenSizeChange;
	public static PlayModeStateChange stateChange { get; set; }


	private static bool staticAwakeAdded;
	/// <summary>
	/// Called once per class on game and editor awake, this should be assigned a static method
	/// </summary>
	protected virtual Action onStaticAwake { get; set; }
	protected CustomMono()
	{
		CustomMonoHelper.onAssign += TryAssign;
		CustomMonoHelper.onEditorAwake += TryOnEditorAwake;

		if (staticAwakeAdded) { return; }

		CustomMonoHelper.onClassAwake += () => onStaticAwake?.Invoke();
		staticAwakeAdded = true;
	}

	private void TryAssign()
	{

		if (this == null
			|| gameObject == null
			|| string.IsNullOrEmpty(gameObject.scene.name))
		{
			CustomMonoHelper.onAssign -= TryAssign;

			return;
		}

		Assign();

	}
	/// <summary>
	/// Called when entering play mode or edit mode on gameobjects in the scene
	/// </summary>
	protected virtual void Assign() {}
	private void TryOnEditorAwake()
	{
		if (this == null
			|| gameObject == null
			|| string.IsNullOrEmpty(gameObject.scene.name))
		{
			CustomMonoHelper.onEditorAwake -= TryAssign;
			return;
		}
		if (isActiveAndEnabled)
		{
			OnEditorAwake();
		}

	}

	/// <summary>
	/// Called when entering edit mode on objects present in the scene
	/// </summary>
	protected virtual void OnEditorAwake() {}
}

[InitializeOnLoad, DefaultExecutionOrder(-9999)]
public static class CustomMonoHelper
{
	internal static Action onAssign, onEditorAwake, onClassAwake;
	private static HashSet<CustomMono> monos;

	static CustomMonoHelper()
	{
		EditorApplication.playModeStateChanged += onModeChange;
	}
	private static void onModeChange(PlayModeStateChange state)
	{
		CustomMono.playmodeState = state;
		if (state == PlayModeStateChange.EnteredEditMode)
		{
			EditorAwake();
		}
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void PlayAwake()
	{
		// Debug.Log("Player awake");
		onAssign?.Invoke();
		onClassAwake?.Invoke();
	}

	[InitializeOnLoadMethod]
	private static void onLoad()
	{
		if (!Application.isPlaying
			&& !EditorApplication.isPlayingOrWillChangePlaymode
			&& !EditorApplication.isCompiling)
		{
			EditorAwake();
		}

	}
	private static void EditorAwake()
	{
		// Debug.Log("Editor awake");
		onAssign?.Invoke();
		onClassAwake?.Invoke();
		onEditorAwake?.Invoke();
	}

}