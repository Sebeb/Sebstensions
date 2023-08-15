using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-200)]
public class DebugHUD : SingletonMonoBehaviour<DebugHUD>
{
	public float fadeTime = 0.5f;

	[Header("References")]
	public TextMeshProUGUI cornerTmp;
	public TextMeshProUGUI centerTmp;
	public CanvasGroup centerCanvasGroup;

	private Dictionary<int, Coroutine> cornerRemoveCoros = new();

	[ClearOnReload(true)]
	private static SortedDictionary<int, string> debugTexts = new();
	[ClearOnReload(true)]
	private static List<string> centerTexts = new();

	private void Start()
	{
		_i.cornerTmp.enabled = PlayerPrefs.GetInt("showDebugCorner", 1) == 1;
	}

	[Button]
	private void TestCenter()
	{
		AddCenterText("Test " + centerTexts.Count);
	}

	public static void AddCornerText(int key, string text, float removeTime = -1,
		bool allowInEditor = false)
	{
		if (!Application.isPlaying && !allowInEditor
		    || _i == null
		    || _i.cornerTmp == null)
		{
			Debug.Log(text);
			return;
		}

		if (debugTexts.ContainsKey(key) && debugTexts[key] == text)
		{
			return;
		}

		debugTexts[key] = text;
		RedrawText();

		if (removeTime > 0)
		{
			_i.CornerTimedRemove(key, removeTime);
		}
	}


	private void CornerTimedRemove(int key, float time)
	{
		if (cornerRemoveCoros.ContainsKey(key))
		{
			StopCoroutine(cornerRemoveCoros[key]);
		}

		cornerRemoveCoros[key] = StartCoroutine(Coro(key, time));

		IEnumerator Coro(int key, float time)
		{
			yield return new WaitForSeconds(time);

			RemoveDebugText(key);
			cornerRemoveCoros.Remove(key);
		}
	}

	public static void AddCenterText(string text, float time = 1.5f)
	{
		centerTexts.Remove(text);
		centerTexts.Add(text);
		UpdateCenterText();
		_i.CenterFade(text, time);
	}

	private Action newCenterText;

	private static void UpdateCenterText()
	{
		_i.centerTmp.text = centerTexts.Join("\n");
		Canvas.ForceUpdateCanvases();
	}
	private void CenterFade(string text, float stayTime, bool mirrorInCosole = true)
	{
		if (mirrorInCosole) Debug.Log(text);

		newCenterText?.Invoke();
		centerCanvasGroup.gameObject.SetActive(true);
		centerCanvasGroup.alpha = 1;
		StartCoroutine(Coro());

		IEnumerator Coro()
		{
			bool fadeOut = true;
			newCenterText += () => fadeOut = false;
			yield return new WaitForSeconds(stayTime);

			while (centerCanvasGroup.alpha > 0 && fadeOut)
			{
				centerCanvasGroup.alpha -= Time.deltaTime / fadeTime;
				yield return null;
			}

			centerTexts.Remove(text);
			
			if (fadeOut) centerCanvasGroup.gameObject.SetActive(false);
			else UpdateCenterText();
		}
	}

	public static void RemoveDebugText(int key)
	{
		if (!debugTexts.ContainsKey(key))
		{
			return;
		}

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