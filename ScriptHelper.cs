using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-110)
#if UNITY_EDITOR
 , ExecuteInEditMode
    #endif
]
public class ScriptHelper : MonoBehaviour
{
    private static ScriptHelper _instance;

    public static ScriptHelper mono
    {
        get
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Coroutines only operational in play-mode");
                return null;
            }
            return _instance;
        }
    }

    public static Coroutine DoCoroutine(IEnumerator routine) => mono.StartCoroutine(routine);

    [
		#if UNITY_EDITOR
        InitializeOnLoadMethod,
		#endif
        RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static ScriptHelper Init()
    {
        if (_instance != null) { return _instance; }

        GameObject runnerObj = GameObject.Find("Script Helper");

        if (runnerObj == null)
        {
            runnerObj = new GameObject("Script Helper");
            runnerObj.AddComponent<ScriptHelper>();
        }
        else
        {
            _instance = runnerObj.GetComponent<ScriptHelper>();
            _instance.Start();
        }

        runnerObj.hideFlags = HideFlags.HideAndDontSave;

        return _instance;
    }

    private void Start() => ScriptableMonoObject.StartMonoScripts();

    private void OnApplicationQuit()
    {
        ScriptableMonoObject.ResetMonoScripts();
        Destroy(gameObject);
    }
}