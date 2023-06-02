using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;


public class DisplayTimeAttributeDrawer : OdinAttributeDrawer<DisplayTimeAttribute, float>
{
	private ValueResolver<Timer.State> stateResolver;

	protected override void Initialize()
	{
		stateResolver = Attribute.GetState.IsNullOrEmpty()
			? null
			: ValueResolver.Get<Timer.State>(Property, Attribute.GetState);
	}

	public static Color StateToColor(Timer.State state)
	{
		bool dark = EditorGUIUtility.isProSkin;
		switch (state)
		{
			case Timer.State.Stopped:
				return dark
					? new Color(1f, 0.22f, 0.25f)
					: new Color(0.57f, 0f, 0f);
			case Timer.State.Paused:
				return dark
					? new Color(1f, 0.53f, 0.26f)
					: new Color(0.64f, 0.17f, 0f);
			case Timer.State.Running:
				return dark
					? new Color(0.49f, 1f, 0.47f)
					: new Color(0f, 0.56f, 0f);

			case Timer.State.Completed:
				return dark
					? new Color(0f, 1f, 0.91f)
					: new Color(0f, 0.05f, 0.61f);

			default:
				throw new ArgumentOutOfRangeException(nameof(state), state, null);
		}
	}

	protected override void DrawPropertyLayout(GUIContent label)
	{
		float time = ValueEntry.SmartValue;
		Rect rect = EditorGUILayout.GetControlRect();
		GUIStyle style = new GUIStyle(GUI.skin.label);
		if (stateResolver != null)
		{
			style.hover.textColor =
				style.focused.textColor =
					style.normal.textColor = StateToColor(stateResolver.GetValue());
		}

		style.alignment = TextAnchor.MiddleRight;
		style.fixedWidth = 75;
		EditorGUI.TextField(rect, label, Timer.ToHhMmSs(time, 0), style);
	}
}

public class DisplayTimerActionsDrawer : OdinAttributeDrawer<DisplayTimerActionsAttribute, SDictionary<float, Action>>
{
	private ValueResolver<float> stateResolver;

	protected override void Initialize()
	{
		stateResolver = Attribute.GetElapsedTime.IsNullOrEmpty()
			? null
			: ValueResolver.Get<float>(Property, Attribute.GetElapsedTime);
	}

	protected override void DrawPropertyLayout(GUIContent label)
	{
		foreach ((float time, Action action) in ValueEntry.SmartValue)
		{
			Rect rect = EditorGUILayout.GetControlRect();
			float elapsed = stateResolver?.GetValue() ?? 0;
			float remaining = elapsed < time ? time - elapsed : 0;
			EditorGUI.ProgressBar(rect, elapsed == 0 ? 1 : elapsed / time, $"{Timer.ToHhMmSs(remaining, 0)} : {action.Method.Name}");
		}
	}
}