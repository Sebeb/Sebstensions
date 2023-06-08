using Sirenix.OdinInspector;
using UnityEngine;


public class SettingsScriptable : ScriptableMonoObject, ICacheable
{
	[SerializeReference, HideLabel]
	public Settings settings;
}