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
	protected static bool quitting => ScriptHelper.quitting;
	[ClearOnReload]
	public static Action OnScreenSizeChange;
}