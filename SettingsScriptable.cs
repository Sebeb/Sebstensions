using Sirenix.OdinInspector;
using UnityEngine;


[HideMonoScript]
public class SettingsScriptable : ScriptableMonoObject, ICacheable
{
	[SerializeReference, HideLabel, InlineProperty, HideReferenceObjectPicker]
	public Settings settings;
}