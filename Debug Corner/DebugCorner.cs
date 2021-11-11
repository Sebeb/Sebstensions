﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-200)]
public class DebugCorner : MonoBehaviour
{
	private static TextMeshProUGUI tmp;
	private static SortedDictionary<int, string> debugTexts = new SortedDictionary<int, string>();
	private Canvas canvas;
	private static DebugCorner instance;

	private void Awake()
	{
		tmp = GetComponent<TextMeshProUGUI>();
		instance = this;
		tmp.enabled = PlayerPrefs.GetInt("showDebugCorner", 1) == 1;
	}

	public static void AddDebugText(int key, string text, float removeTime = -1)
	{
		if (!Application.isPlaying)
		{
			Debug.Log(text);
			return;
		}

		if (debugTexts.ContainsKey(key) && debugTexts[key] == text) { return; }

		debugTexts[key] = text;
		RedrawText();

		if (removeTime > 0)
		{
			instance.StartCoroutine(instance.TimedRemove(key, removeTime));
		}
	}

	private IEnumerator TimedRemove(int key, float time)
	{
		yield return new WaitForSeconds(time);

		RemoveDebugText(key);
	}

	public static void RemoveDebugText(int key)
	{
		if (!debugTexts.ContainsKey(key)) { return; }

		debugTexts.Remove(key);
		RedrawText();
	}

	private static void RedrawText()
	{
		tmp.text = "";
		foreach (string debugText in debugTexts.Values)
		{
			tmp.text += debugText;
			tmp.text += "\n";
		}
	}

	private void Update()
	{
		//Toggle visible with F2
		if (UnityEngine.InputSystem.Keyboard.current.f2Key.wasPressedThisFrame)
		{
			tmp.enabled = !tmp.enabled;
			PlayerPrefs.SetInt("showDebugCorner", tmp.enabled.AsInt());

		}
		// if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows
		// 	&& Keyboard.current.f1Key.wasReleasedThisFrame)
		// {
		// 	StartCoroutine(TakeScreenshot());
		// }
	}

	// private IEnumerator TakeScreenshot()
	// {
	// 	bool debugDisplayed = tmp.enabled;
	// 	if (debugDisplayed)
	// 	{
	// 		tmp.enabled = false;
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
	// 	if (debugDisplayed) { tmp.enabled = true; }
	// 	Game.cursor.Enable();
	// }
}