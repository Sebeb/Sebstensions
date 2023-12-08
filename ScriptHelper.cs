using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(-110), ExecuteAlways]
public class ScriptHelper : MonoBehaviour
{
	public static bool quitting { get; private set; }
	private static ScriptHelper _instance;
	private Vector2 screenSize;
	public static float frameStartTime { get; private set; }
	public static float realtimeFrameTime => Time.realtimeSinceStartup - frameStartTime;

	public static bool retryDreamLoad = false;

	public static ScriptHelper mono
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

	public static Coroutine StartCoroutine(IEnumerator routine, bool allowInEditor = false, bool silent = false)
	{
		if (!Application.isPlaying && !allowInEditor)
		{
			if (!silent) Debug.LogError("Coroutine only operational in play-mode");
			return null;
		}
		return ((MonoBehaviour)mono).StartCoroutine(routine);
	}
	public static void StopCoroutine(Coroutine routine) => ((MonoBehaviour)mono).StopCoroutine(routine);

	public static Coroutine WaitTimeThenExecute(float seconds, Action action,
		Action<float, float> timeOut = null, bool useScaledTime = false)
	{
		return StartCoroutine(Coroutine());
		IEnumerator Coroutine()
		{
			float CurrentTime() => useScaledTime ? Time.time : Time.unscaledTime;
			float endTime = CurrentTime() + seconds;
			while (CurrentTime() < endTime)
			{
				float remainingTime = endTime - Time.time;
				timeOut?.Invoke(remainingTime, 1 - (remainingTime / seconds));
				yield return null;
			}
			timeOut?.Invoke(0, 1);
			action?.Invoke();
		}
	}

	public static Coroutine WaitTimeThenExecute(Func<bool> waitUntilTrue, Action action, Action waitingAciton = null)
	{
		return StartCoroutine(Coroutine());
		IEnumerator Coroutine()
		{
			while (waitUntilTrue?.Invoke() == false)
			{
				waitingAciton?.Invoke();
				yield return null;
			}
			action?.Invoke();
		}
	}

	public static Coroutine WaitFramesThenExecute(int frames, Action action, bool allowInEditor = false)
	{
		return StartCoroutine(Coroutine(), allowInEditor);
		IEnumerator Coroutine()
		{
			while (frames-- > 0)
			{
				yield return null;
			}

			action?.Invoke();
		}
	}

	/// <summary>
	/// Determines whether custom ScriptableMonoObjects should be updated and started, as well as whether MonoObject Singletons should be dynamically created.
	/// </summary>
	public static bool foundationEnabled { get; private set; } = true;
	private static bool fakeStart;

#if UNITY_EDITOR
	[InitializeOnLoadMethod, UnityEditor.Callbacks.DidReloadScripts]
	private static void OnModeChange()
	{
		if (!Application.isPlaying
		    && !EditorApplication.isPlayingOrWillChangePlaymode
		    && _instance == null)
		{
			OnModeChange(PlayModeStateChange.EnteredEditMode);
		}

		EditorApplication.playModeStateChanged -= OnModeChange;
		EditorApplication.playModeStateChanged += OnModeChange;
	}

	private static void OnModeChange(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode)
		{
			_instance = FindObjectOfType<ScriptHelper>();
			if (_instance == null) SetInstance();
		}
	}
#endif

	public static void SetInstance(bool runCallbacks = true)
	{
		ScriptHelper.foundationEnabled = runCallbacks;
		if (_instance != null)
		{
			Debug.Log("Destroying existing Script Helper", _instance);
			_instance.gameObject.Destroy(true);
			_instance = null;
		}

		var runnerObj = new GameObject("Script Helper");
		_instance = runnerObj.AddComponent<ScriptHelper>();
		Debug.Log("Created new Script Helper");

		fakeStart = Application.isPlaying && Time.frameCount != 0;
		if (fakeStart) Debug.Log("Faking game start");

		runnerObj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
		if (Application.isPlaying) DontDestroyOnLoad(runnerObj);

		Application.quitting -= OnApplicationQuitting;
		Application.quitting += OnApplicationQuitting;
	}
	private static void OnApplicationQuitting() => quitting = true;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void FakeStart()
	{
		if (!fakeStart) return;

		fakeStart = false;
		_instance.Start();
	}


	private void Update()
	{
		frameStartTime = Time.realtimeSinceStartup;
		if (Seb.screenSize != screenSize)
		{
			screenSize = Seb.screenSize;
			CustomMono.OnScreenSizeChange?.Invoke();
		}
		if (foundationEnabled) ScriptableMonoObject.UpdateMonoScripts();
	}

	private void Start()
	{
		if (foundationEnabled) ScriptableMonoObject.StartMonoScripts();
		CustomMono.OnScreenSizeChange?.Invoke();
	}
	public static bool FrameIsSlow(float targetFps, bool playModeOnly = true, bool printDebug = false)
	{
		if ((Application.isPlaying || !playModeOnly) && targetFps <= 0 || realtimeFrameTime > 1.0f / targetFps)
		{
			if (printDebug) { print("Waiting frame"); }
			return true;
		}
		return false;
	}

	public static void DeInit(bool isRestart)
	{
		if (foundationEnabled) ScriptableMonoObject.ResetMonoScripts(isRestart);
		if (_instance != null) _instance.gameObject.Destroy(immediate: true);
	}
	private void OnApplicationQuit()
	{
		quitting = true;
		DeInit(false);
	}
}