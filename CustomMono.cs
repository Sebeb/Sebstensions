using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


public abstract class CustomMono : SerializedMonoBehaviour
{
#if UNITY_EDITOR
	[FoldoutGroup("Settings"), ShowInInspector, LabelText("Settings"), ShowIf("GetSettings"),
	 OnInspectorInit("SetSettings"),
	 InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden, DrawHeader = true)]
	private ScriptableMonoObject serializedSettings;

	private void SetSettings()
	{
		if (serializedSettings == null)
		{
			serializedSettings = GetSettings();
		}
	}

	private ScriptableMonoObject GetSettings() => Managers.GetAssociated(GetType(), silent: true);
#endif
	protected static bool quitting => ScriptHelper.quitting;
	[ClearOnReload]
	public static Action OnScreenSizeChange;


	[ClearOnReload]
	internal protected static Map<Type, Action> onStaticAwake = new();


	protected CustomMono()
	{
		AssignDelegates();
	}

	internal void AssignDelegates()
	{
		// Debug.Log("Assigning delegates");
		CustomMonoHelper.OnAssign += TryAssign;
		CustomMonoHelper.OnEditorAwake += TryOnEditorAwake;
	}


	private void TryAssign()
	{
		if (this == null
		    || gameObject == null
		    || string.IsNullOrEmpty(gameObject.scene.name))
		{
			CustomMonoHelper.OnAssign -= TryAssign;
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
			CustomMonoHelper.OnEditorAwake -= TryAssign;
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
	[ClearOnReload]
	internal static Action OnAssign, OnEditorAwake;
	private static HashSet<CustomMono> monos;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void PlayAwake()
	{
		IEnumerable<Type> customMonoClasses = Reflection.GetAllScriptChildTypes<CustomMono>();

		IEnumerable<MethodInfo> methods =
			customMonoClasses
				.SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static))
				.Where(m => m.Name == "ClassAwake");


		foreach (CustomMono customMono in Object.FindObjectsOfType<CustomMono>(true))
		{
			customMono.AssignDelegates();
		}

		OnAssign?.Invoke();

		methods.ForEach(m => m.Invoke(null, null));
	}

#if UNITY_EDITOR
	static CustomMonoHelper()
	{
		EditorApplication.playModeStateChanged += OnModeChange;
	}

	private static void OnModeChange(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode)
		{
			EditorAwake();
		}
		else if (state == PlayModeStateChange.ExitingPlayMode)
		{
			OnAssign = null;
			OnEditorAwake = null;
		}
	}

	[InitializeOnLoadMethod]
	private static void OnLoad()
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
		OnAssign?.Invoke();
		OnEditorAwake?.Invoke();
	}

}