using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-200)]
public class DebugHUD : SingletonMonoBehaviour<DebugHUD>
{
	public Vector2 centerStayFadeTime;

	[Header("References")]
	public TextMeshProUGUI cornerTmp;
	public TextMeshProUGUI centerTmp;
	public CanvasGroup centerCanvasGroup;

	private Dictionary<int, Coroutine> cornerRemoveCoros = new();
	private Coroutine centerFadeCoro;

	private static SortedDictionary<int, string> debugTexts = new();

	private void Start()
	{
		_i.cornerTmp.enabled = PlayerPrefs.GetInt("showDebugCorner", 1) == 1;
	}

	public static void AddCornerText(int key, string text, float removeTime = -1,
		bool allowInEditor = false)
	{
		if (!Application.isPlaying && !allowInEditor
		    || _i == null || _i.cornerTmp == null)
		{
			Debug.Log(text);
			return;
		}

		if (debugTexts.ContainsKey(key) && debugTexts[key] == text) { return; }

		debugTexts[key] = text;
		RedrawText();

		if (removeTime > 0)
		{
			_i.CornerTimedRemove(key, removeTime);
		}
	}

	public static void AddCenterText(string text)
	{
		_i.centerTmp.text = text;
		_i.CenterFade();
	}

	private void CornerTimedRemove(int key, float time)
	{
		if (cornerRemoveCoros.ContainsKey(key)) { StopCoroutine(cornerRemoveCoros[key]); }

		cornerRemoveCoros[key] = StartCoroutine(Coro(key, time));

		IEnumerator Coro(int key, float time)
		{
			yield return new WaitForSeconds(time);

			RemoveDebugText(key);
			cornerRemoveCoros.Remove(key);
		}
	}

	private void CenterFade()
	{
		centerCanvasGroup.gameObject.SetActive(true);
		centerCanvasGroup.alpha = 1;
		if (centerFadeCoro != null) { StopCoroutine(centerFadeCoro); }
		centerFadeCoro = StartCoroutine(Coro());

		IEnumerator Coro()
		{
			yield return new WaitForSeconds(centerStayFadeTime.x);

			while (centerCanvasGroup.alpha > 0)
			{
				centerCanvasGroup.alpha -= Time.deltaTime / centerStayFadeTime.y;
			}

			centerCanvasGroup.gameObject.SetActive(false);
			centerFadeCoro = null;
		}
	}

	public static void RemoveDebugText(int key)
	{
		if (!debugTexts.ContainsKey(key)) { return; }

		debugTexts.Remove(key);
		RedrawText();
	}

	private static void RedrawText()
	{
		_i.cornerTmp.text = "";
		foreach (string debugText in debugTexts.Values)
		{
			_i.cornerTmp.text += debugText;
			_i.cornerTmp.text += "\n";
		}
	}

	private void Update()
	{
		//Toggle visible with F2
		if (Keyboard.current.f2Key.wasPressedThisFrame)
		{
			cornerTmp.enabled = !cornerTmp.enabled;
			PlayerPrefs.SetInt("showDebugCorner", cornerTmp.enabled.AsInt());

		}
		// if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows
		// 	&& Keyboard.current.f1Key.wasReleasedThisFrame)
		// {
		// 	StartCoroutine(TakeScreenshot());
		// }
	}

	// private IEnumerator TakeScreenshot()
	// {
	// 	bool debugDisplayed = _i.cornerTmp.enabled;
	// 	if (debugDisplayed)
	// 	{
	// 		_i.cornerTmp.enabled = false;
	// 		yield return null;
	// 	}
	// 	Game.cursor.Disable();
	//
	// 	yield return null;
	//
	// 	string folderPath = GameSettings._i.screenshotLocation;
	// 	// string.Concat(
	// 	// Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).Replace("\\", "/"), '/',
	// 	// "PES Screenshots/");
	//
	// 	Seb.EnsureFolderExists(folderPath);
	//
	// 	ScreenCapture.CaptureScreenshot(
	// 		$"{folderPath}\\PES {Game.dreamKeyword.MakeFilename(20)} {DateTime.Now.Day}_{DateTime.Now.Month}_{DateTime.Now.Year.ToString().Substring(2)}.png"
	// 			.GetIncrementalFileNumber(true));
	//
	// 	yield return new WaitForSeconds(0.5f);
	//
	// 	if (debugDisplayed) { _i.cornerTmp.enabled = true; }
	// 	Game.cursor.Enable();
	// }
}