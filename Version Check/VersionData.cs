using UnityEngine;


public class VersionData : SingletonScriptableObject<VersionData>
{
	public string fullVersion => $"{Application.version}.{_i.versionBuildNo}";
	public int versionBuildNo;
	public int cloudBuildNo = -1;
}