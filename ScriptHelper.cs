using System.Collections;
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
    private Vector2 screenSize;

    public static ScriptHelper mono
    {
        get
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Coroutines only operational in play-mode");
                return null;
            }
            _instance ??= Init();
            return _instance;
        }
    }

    public static Coroutine DoCoroutine(IEnumerator routine)
    {
        return mono.StartCoroutine(routine);
    }

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

    private void Update()
    {
        if (Seb.screenSize != screenSize)
        {
            screenSize = Seb.screenSize;
            CustomMono.OnScreenSizeChange?.Invoke();
        }
    }
    private void Start() => ScriptableMonoObject.StartMonoScripts();

    private void OnApplicationQuit()
    {
        ScriptableMonoObject.ResetMonoScripts();
        Destroy(gameObject);
    }
}