using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Weaver;

public class VersionExporter : UnityEditor.AssetModificationProcessor, IPreprocessBuildWithReport
{
	public static string[] OnWillSaveAssets(string[] paths)
	{
		if (!paths.Contains("ProjectSettings/ProjectSettings.asset")) { return paths; }

		StreamWriter writer = new StreamWriter(Application.dataPath + "/version.txt", false,
			new UTF8Encoding(true));
		writer.WriteLine(Application.version);

		writer.Close();
		// SetAppNameBuildNo(Game.settings.buildCount, false);

		return paths;
	}

	public int callbackOrder => 0;
//ON LOCAL BUILD
	public void OnPreprocessBuild(BuildReport report)
	{
	#if !UNITY_CLOUD_BUILD
		SetAppNameBuildNo(BuildDetails.BuildNumber, true);
	#endif
	}

//ON CLOUD BUILD
#if UNITY_CLOUD_BUILD
	public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest)
	{
		Game.settings.buildCount = manifest.GetValue<int>("buildNumber");
		SetAppNameBuildNo(manifest.GetValue<int>("buildNumber"), false);
		Debug.Log($"Set name according to build count {Game.settings.buildCount}");
	}
#endif

	public static void SetAppNameBuildNo(int buildNo, bool local)
	{
		// Game.settings.buildCount = buildNo;
		// Game.settings.buildVersion = PlayerSettings.bundleVersion =
			// $"{Application.version}.{buildNo}{(local ? 'l' : 'c')}";
	}

}