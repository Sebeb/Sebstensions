using Sirenix.OdinInspector;
using UnityEngine;


public class SettingsScriptable : ScriptableMonoObject, ICacheable
{
	[SerializeReference, HideLabel, InlineProperty, HideReferenceObjectPicker]
	public Settings settings;
}