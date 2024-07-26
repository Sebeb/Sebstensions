using System;
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
	private static Dictionary<int, Coroutine> activeCountDowns = new Dictionary<int, Coroutine>();

	private void Awake()
	{
		tmp = GetComponent<TextMeshProUGUI>();
		instance = this;
		tmp.enabled = PlayerPrefs.GetInt("showDebugCorner", 1) == 1;
	}

	public static void AddDebugText(int key, string text, float removeTime = -1, string color = null,
		bool mirrorToConsole = false)
	{
		if (!Application.isPlaying)
		{
			Debug.Log(text);
			return;
		}

		if (debugTexts.ContainsKey(key) && debugTexts[key] == text)
		{
			return;
		}

		if (color != null)
		{
			text = $"<color={color}>" + text + "</color>";
			// text = $"<color={color.Value.AsHex()}>" + text + "</color>";
		}

		debugTexts[key] = text;
		RedrawText();

		if (removeTime > 0)
		{
			if (activeCountDowns.ContainsKey(key))
			{
				instance.StopCoroutine(activeCountDowns[key]);
				activeCountDowns.Remove(key);
			}

			activeCountDowns.Add(key, instance.StartCoroutine(instance.TimedRemove(key, removeTime)));
		}
	#if UNITY_EDITOR || DEVELOPMENT_BUILD
		if (mirrorToConsole)
		{
			Debug.Log(text);
		}
	#endif
	}

	private IEnumerator TimedRemove(int key, float time)
	{
		yield return new WaitForSeconds(time);

		RemoveDebugText(key);
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
		if (Keyboard.current.f2Key.wasPressedThisFrame)
		{
			tmp.enabled = !tmp.enabled;
			PlayerPrefs.SetInt("showDebugCorner", tmp.enabled.AsInt());
		}
	}
	
}