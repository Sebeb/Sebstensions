using System;
using System.Collections.Generic;
using System.Linq;
using UnityCloudBuildAPI;
using UnityEngine;

public class VersionData : SingletonScriptableObject<VersionData>
{
	public string fullVersion => $"{Application.version}.{_i.versionBuildNo}";
	public int versionBuildNo;
	public int cloudBuildNo = -1;

	public VersionChangelog[] changelogs;

	[Serializable]
	public struct VersionChangelog
	{
		public string baseVersion;
		public int buildNo, versionBuildNo;
		public string[] changes;
		public string getVersionName => $"{baseVersion}.{buildNo}";

		public static VersionChangelog[] FromCloudBuilds(CloudBuild[] builds)
		{
			List<VersionChangelog> log = new List<VersionChangelog>();
			string currentBaseVersion = "";

			foreach (CloudBuild build in builds.Where(b =>
				b.Deleted == false && b.BuildStatus == "success"))
			{
				VersionChangelog currentLog = new VersionChangelog
					{ buildNo = (int)build.Build };
				List<string> changes = new List<string>();
				foreach (Changeset changeset in build.Changeset)
				{
					if (string.IsNullOrEmpty(changeset.Message)) { continue; }

					string[] commitMsgLines = changeset.Message.Split('\n');
					for (int i = 0; i < commitMsgLines.Length; i++)
					{
						string commitMsg = commitMsgLines[i];
						if (string.IsNullOrEmpty(commitMsg)) { continue; }

						if (i == 0)
						{
							if (commitMsg.FirstOrDefault() == '(' &&
								commitMsg.LastOrDefault() == ')')
							{
								continue;
							}

							if (!commitMsg.Contains(' ') && commitMsg.Contains('.'))
							{
								currentLog.baseVersion = commitMsg.Split(' ').First();
								continue;
							}
						}

						if (commitMsg.FirstOrDefault() != '~') { changes.Add(commitMsg); }
					}
				}
				currentLog.changes = changes.ToArray();
				if (string.IsNullOrEmpty(currentLog.baseVersion) && log.Any())
				{
					currentLog.baseVersion = log.Last().baseVersion;
				}
				if (log.Any() && log.Last().baseVersion == currentLog.baseVersion)
				{
					currentLog.versionBuildNo = log.Last().versionBuildNo + 1;
				}
				else { currentLog.versionBuildNo = 0; }
				log.Add(currentLog);
			}

			return log.ToArray();
		}
	}
}