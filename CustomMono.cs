using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class CustomMono : MonoBehaviour
{
	public static Action OnScreenSizeChange;

	/// <summary>
	/// Called once per class on game and editor awake, this should be assigned a static method
	/// </summary>
	protected virtual Action onStaticAwake { get; set; }
	protected CustomMono()
	{
		CustomMonoHelper.onAssign += TryAssign;
		CustomMonoHelper.onEditorAwake += TryOnEditorAwake;
		CustomMonoHelper.onClassAwake += () => onStaticAwake?.Invoke();
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

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DefaultExecutionOrder(-9999)]
public static class CustomMonoHelper
{
	internal static Action onAssign, onEditorAwake, onClassAwake;
	private static HashSet<CustomMono> monos;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void PlayAwake()
	{
		Debug.Log("Player awake");
		onAssign?.Invoke();
		onClassAwake?.Invoke();
	}

#if UNITY_EDITOR
	static CustomMonoHelper()
	{
		EditorApplication.playModeStateChanged += onModeChange;
	}
	private static void onModeChange(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode)
		{
			EditorAwake();
		}
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
#endif
	private static void EditorAwake()
	{
		// Debug.Log("Editor awake");
		onAssign?.Invoke();
		onEditorAwake?.Invoke();
	}

}