using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

[DefaultExecutionOrder(-110), ExecuteInEditMode]
public class ScriptHelper : MonoBehaviour
{
	public static bool quitting { get; private set; }
	private static ScriptHelper _instance;
	private Vector2 screenSize;
	public static float frameStartTime { get; private set; }
	public static float realtimeFrameTime => Time.realtimeSinceStartup - frameStartTime;

	public static ScriptHelper mono
	{
		get
		{
			if (!Application.isPlaying)
			{
				Debug.LogError("Coroutines only operational in play-mode");
				return null;
			}
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

	public static Coroutine WaitThenExecute(float time, Action action,
		Action<float, float> timeOut = null)
	{
		return StartCoroutine(Coroutine());
		IEnumerator Coroutine()
		{
			float endTime = Time.time + time;
			while (Time.time < endTime)
			{
				float remainingTime = endTime - Time.time;
				timeOut?.Invoke(remainingTime, 1 - (remainingTime / time));
				yield return null;
			}
			timeOut?.Invoke(0, 1);
			action?.Invoke();
		}
	}

	public static Coroutine WaitThenExecute(Func<bool> waitUntilTrue, Action action)
	{
		return StartCoroutine(Coroutine());
		IEnumerator Coroutine()
		{
			while (waitUntilTrue?.Invoke() == false)
			{
				yield return null;
			}
			action?.Invoke();
		}
	}
	
	public static Coroutine WaitThenExecute(int frames, Action action, bool allowInEditor = false)
	{
		return StartCoroutine(Coroutine(), allowInEditor);
		IEnumerator Coroutine()
		{
			yield return frames;

			action?.Invoke();
		}
	}


	private static bool fakeStart;

#if UNITY_EDITOR
	[InitializeOnLoadMethod]
	private static void SetInstanceEditor()
	{
		Debug.Log(Application.isPlaying);
		if (!Application.isPlaying) return;
		SetInstance();
	}
#endif
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void SetInstance()
	{
		if (_instance != null) return;

		GameObject runnerObj = GameObject.Find("Script Helper");

		if (runnerObj != null)
		{
			_instance = runnerObj.GetComponent<ScriptHelper>();
		}
		if (runnerObj == null)
		{
			runnerObj = new GameObject("Script Helper");
			runnerObj.AddComponent<ScriptHelper>();
		}
		fakeStart = Time.frameCount != 0;
		if (fakeStart) Debug.Log("Faking game start");

		runnerObj.hideFlags = HideFlags.DontSave;
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
		ScriptableMonoObject.UpdateMonoScripts();
		frameStartTime = Time.realtimeSinceStartup;
		if (Seb.screenSize != screenSize)
		{
			screenSize = Seb.screenSize;
			CustomMono.OnScreenSizeChange?.Invoke();
		}
	}
	private void Start()
	{
		ScriptableMonoObject.StartMonoScripts();
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
		ScriptableMonoObject.ResetMonoScripts(isRestart);
		_instance.gameObject.Destroy(immediate: true);
	}
	private void OnApplicationQuit()
	{
		quitting = true;
		DeInit(false);
	}
}