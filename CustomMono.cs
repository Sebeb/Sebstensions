using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


public abstract class CustomMono : SerializedMonoBehaviour
{
#if UNITY_EDITOR
	[FoldoutGroup("Settings"), ShowInInspector, LabelText("Settings"), ShowIf("GetSettings"),
	 OnInspectorInit("SetSettings"),
	 InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden, DrawHeader = true)]
	private ScriptableMonoObject serializedSettings;

	private void SetSettings()
	{
		if (serializedSettings == null)
		{
			serializedSettings = GetSettings();
		}
	}

	private ScriptableMonoObject GetSettings() => Managers.GetAssociated(GetType(), silent: true);
#endif
	protected static bool quitting => ScriptHelper.quitting;
	[ClearOnReload]
	public static Action OnScreenSizeChange;
}