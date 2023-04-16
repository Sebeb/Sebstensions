using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEditor;


[ShowOdinSerializedPropertiesInInspector]
public abstract class CustomMono : MonoBehaviour, ISerializationCallbackReceiver, ISupportsPrefabSerialization
{
	protected static bool quitting => ScriptHelper.quitting;
	public static Action OnScreenSizeChange;

	/// <summary>
	/// Called once per class on game and editor awake, this should be assigned a static method
	/// </summary>
	protected virtual Action onStaticAwake { get; set; }
	protected CustomMono()
	{
		CustomMonoHelper.OnAssign += TryAssign;
		CustomMonoHelper.OnEditorAwake += TryOnEditorAwake;
		CustomMonoHelper.OnClassAwake += () => onStaticAwake?.Invoke();
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

	#region Odin

	[SerializeField, HideInInspector]
	private SerializationData serializationData;

	SerializationData ISupportsPrefabSerialization.SerializationData
	{
		get { return this.serializationData; }
		set { this.serializationData = value; }
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		UnitySerializationUtility.DeserializeUnityObject(this, ref this.serializationData);
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData);
	}

	#endregion

}

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DefaultExecutionOrder(-9999)]
public static class CustomMonoHelper
{
	internal static Action OnAssign, OnEditorAwake, OnClassAwake;
	private static HashSet<CustomMono> monos;


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void PlayAwake()
	{
		// Debug.Log("Player awake");
		OnAssign?.Invoke();
		OnClassAwake?.Invoke();
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