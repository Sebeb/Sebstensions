using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
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

	public static Color StateToColor(Timer.State state) => StateToColor(state, EditorGUIUtility.isProSkin);
	public static Color StateToColor(Timer.State state, bool bright)
	{
		switch (state)
		{
			case Timer.State.Stopped:
				return bright
					? new Color(1f, 0.22f, 0.25f)
					: new Color(0.57f, 0f, 0f);
			case Timer.State.Paused:
				return bright
					? new Color(1f, 0.53f, 0.26f)
					: new Color(0.64f, 0.17f, 0f);
			case Timer.State.Running:
				return bright
					? new Color(0.49f, 1f, 0.47f)
					: new Color(0f, 0.44f, 0f);

			case Timer.State.Completed:
				return bright
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
		GUIStyle style = new GUIStyle(GUI.skin.textField);
		if (stateResolver != null)
		{
			style.normal.textColor = StateToColor(stateResolver.GetValue());
		}

		style.alignment = TextAnchor.MiddleRight;
		style.fixedWidth = 75;
		
		ValueEntry.SmartValue = Timer.FromHhMmSs(EditorGUI.TextField(rect, label, Timer.ToHhMmSs(time, Attribute.decimalPlaces), style));
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

			// Draw the health bar.
			float width = Mathf.Clamp01(elapsed / time);
			SirenixEditorGUI.DrawSolidRect(rect, new Color(0f, 0f, 0f, 0.3f), false);
			SirenixEditorGUI.DrawSolidRect(rect.SetWidth(rect.width * width), DisplayTimeAttributeDrawer.StateToColor(remaining > 0 ? Timer.State.Running : Timer.State.Completed, false), false);
			
			//Label remaining time
			var style = new GUIStyle(GUI.skin.label)
			{
				alignment = TextAnchor.MiddleRight,
				normal =
				{
					textColor = Color.white
				},
				fontSize = 10,
				fontStyle = FontStyle.Bold
			};
			EditorGUI.LabelField(rect, (remaining > 0 ? Timer.ToHhMmSs(remaining, 0) : "Completed!") + "  ", style) ;
			style.alignment = TextAnchor.MiddleLeft;
			EditorGUI.LabelField(rect, $"{Timer.ToHhMmSs(time, 0)} : {action.Method.Name}", style);

			SirenixEditorGUI.DrawBorders(rect, 1);
		}
	}
}