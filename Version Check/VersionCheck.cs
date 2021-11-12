using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using BestHTTP;
using NaughtyAttributes;
using TMPro;
using UnityCloudBuildAPI;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public partial class VersionCheck : SingletonMonoBehaviour<VersionCheck>
{
	static VersionData data => VersionData._i;
	public bool checkItchUpdates, checkCloudBuildUpdates;
	private string updateDownloadUrl;

	[Header("Unity Cloud Build")]
	[ShowIf("checkCloudBuildUpdates")] public long orgId;
	[ShowIf("checkCloudBuildUpdates")] public string projectId, targetName;

	[ShowIf("checkItchUpdates")]
	public string itchUpdateUrl =
		"https://itch.io/api/1/x/wharf/latest?game_id=635733&channel_name=";

	private string latestVersion, currentVersion;


	private void Start()
	{
		DebugCorner.AddDebugText(-11, data.fullVersion, allowInEditor: true);
		if (checkItchUpdates) { StartCoroutine(ItchUpdatesCheck()); }
		if (checkCloudBuildUpdates) { UnityCloudUpdatesCheck(); }
	}

	[Button]
	private void UnityCloudUpdatesCheck()
	{
		HTTPRequest req = new HTTPRequest(
			new Uri(
				$"https://build-api.cloud.unity3d.com/api/v1/orgs/{orgId}/projects/{projectId}/buildtargets/{targetName}/builds"),
			onReceiveBuilds);

		req.AddHeader("Content-Type", "application/json");
		req.AddHeader("Authorization", "Basic e8b2369b3ec2b0e69fd4273a70e4f79a");
		req.Send();

		DebugCorner.AddDebugText(-10, "Checking cloud build for updates...");

		void onReceiveBuilds(HTTPRequest req, HTTPResponse resp)
		{
			if (!resp.IsSuccess)
			{
				DebugCorner.AddDebugText(-10, "Error checking cloud build for updates", 5);
			}

			CloudBuild[] builds;
			try
			{
				builds = CloudBuild.FromJson(resp.DataAsText);

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				Debug.Log(resp.DataAsText);
				throw;
			}
			if (!builds.Any()) { return; }

			builds = builds.OrderByDescending(b => b.Created).ToArray();
			print(
				"All available builds: " +
				string.Join("\n", builds.Select(b => b.ToString())));


			data.changelogs = VersionData.VersionChangelog.FromCloudBuilds(builds);

		#if UNITY_EDITOR
			if (data.changelogs.Any())
			{
				data.versionBuildNo = data.changelogs.Last().baseVersion == Application.version
					? data.changelogs.Last().buildNo + 1 : 1;
			}
			ExportVersion();
		#endif

			CloudBuild latestSuccessfulBuild =
				builds.FirstOrDefault(b => b.BuildStatus == "success");

			if (latestSuccessfulBuild == null) { return; }
			print($"Latest successful build: {latestSuccessfulBuild}");

			if (data.cloudBuildNo < (int)latestSuccessfulBuild.Build)
			{
				DebugCorner.AddDebugText(-10,
					"Update available. Press enter to open download link");
				updateDownloadUrl = latestSuccessfulBuild.Links.DownloadPrimary.Href.ToString();
			}
			else if (data.cloudBuildNo == (int)latestSuccessfulBuild.Build)
			{
				DebugCorner.AddDebugText(-10, "Up to date!", 3);
			}
			else
			{
				DebugCorner.AddDebugText(-10, "In the future.", 3);
			}
			DebugCorner.AddDebugText(-11, data.fullVersion, allowInEditor: true);
		}
	}

#if UNITY_CLOUD_BUILD
	public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest)
	{
		data.cloudBuildNo = manifest.GetValue<int>("buildNumber");

		StreamReader reader = new StreamReader(Application.dataPath + "/version~",
			new UTF8Encoding(true));
		string lastVersion = reader.ReadLine();
		if (Application.version == lastVersion)
		{
			data.versionBuildNo = int.Parse(reader.ReadLine());
		}
		else { data.versionBuildNo = 1; }
		print($"Version number set to {data.versionBuildNo}");
	}
#endif

	public static void ExportVersion()
	{
		StreamWriter writer = new StreamWriter(Application.dataPath + "/version~", false,
			new UTF8Encoding(true));

		if (!data.changelogs.Any())
		{
			writer.WriteLine(Application.version);
			writer.WriteLine('0');
		}
		else
		{
			string latestVersion = data.changelogs.Last().baseVersion;
			writer.WriteLine(latestVersion);
			writer.WriteLine(data.changelogs.First(b => b.baseVersion == latestVersion).buildNo);
		}
		writer.Close();
	}

	private IEnumerator ItchUpdatesCheck()
	{
		string platform = Application.platform == RuntimePlatform.WindowsEditor
			|| Application.platform == RuntimePlatform.WindowsPlayer
				? "windows-x64"
				: "macos";

		WWW download = new WWW(itchUpdateUrl + platform);
		yield return download;

		if (string.IsNullOrEmpty(download.error))
		{

			DebugCorner.AddDebugText(-11, Application.version + " - checking for updates...");
			string[] rawVersion = download.text.Split('"');

			if (!rawVersion[1].Equals("latest"))
			{
				Debug.Log("Error checking for updates\n" + download.text);
				yield break;
			}

			latestVersion = rawVersion[3];

			if (latestVersion != Application.version)
			{
				print(latestVersion);
				DebugCorner.AddDebugText(-11, Application.version
					+ "\nversion " + latestVersion + " available.");
				yield break;
			}
			else
			{
				DebugCorner.AddDebugText(-11, Application.version + " - up-to-date.");
			}
		}
		else
		{
			DebugCorner.AddDebugText(-11, Application.version + " - error checking for updates.");
		}

		yield return new WaitForSeconds(5);
	}

	public void GoToWebsite()
	{
		Application.OpenURL("https://shytea.itch.io/project-electric-sheep");
	}

	private void Update()
	{
		if (!string.IsNullOrEmpty(updateDownloadUrl)
			&& UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
		{
			Application.OpenURL(updateDownloadUrl);
		}
	}

}

// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using UnityCloudBuildAPI;
//
//    var welcome = Welcome.FromJson(jsonString);

namespace UnityCloudBuildAPI
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public partial class CloudBuild
	{
		public override string ToString() => $"{Created} {Build} {BuildStatus}";
		[JsonProperty("build")]
		public long Build { get; set; }

		[JsonProperty("buildtargetid")]
		public string Buildtargetid { get; set; }

		[JsonProperty("buildTargetName")]
		public string BuildTargetName { get; set; }

		[JsonProperty("buildGUID")]
		public string BuildGuid { get; set; }

		[JsonProperty("buildStatus")]
		public string BuildStatus { get; set; }

		[JsonProperty("cleanBuild")]
		public bool CleanBuild { get; set; }

		[JsonProperty("platform")]
		public string Platform { get; set; }

		[JsonProperty("workspaceSize")]
		public long WorkspaceSize { get; set; }

		[JsonProperty("created")]
		public DateTimeOffset Created { get; set; }

		[JsonProperty("finished")]
		public DateTimeOffset Finished { get; set; }

		[JsonProperty("checkoutStartTime")]
		public DateTimeOffset CheckoutStartTime { get; set; }

		[JsonProperty("checkoutTimeInSeconds")]
		public long CheckoutTimeInSeconds { get; set; }

		[JsonProperty("buildStartTime")]
		public DateTimeOffset BuildStartTime { get; set; }

		[JsonProperty("buildTimeInSeconds")]
		public double BuildTimeInSeconds { get; set; }

		[JsonProperty("publishStartTime")]
		public DateTimeOffset PublishStartTime { get; set; }

		[JsonProperty("publishTimeInSeconds")]
		public double PublishTimeInSeconds { get; set; }

		[JsonProperty("totalTimeInSeconds")]
		public double TotalTimeInSeconds { get; set; }

		[JsonProperty("lastBuiltRevision")]
		public string LastBuiltRevision { get; set; }

		[JsonProperty("changeset")]
		public Changeset[] Changeset { get; set; }

		[JsonProperty("favorited")]
		public bool Favorited { get; set; }

		[JsonProperty("deleted")]
		public bool Deleted { get; set; }

		[JsonProperty("headless")]
		public bool Headless { get; set; }

		[JsonProperty("credentialsOutdated")]
		public bool CredentialsOutdated { get; set; }

		[JsonProperty("queuedReason")]
		public string QueuedReason { get; set; }

		[JsonProperty("cooldownDate")]
		public DateTimeOffset CooldownDate { get; set; }

		[JsonProperty("scmBranch")]
		public string ScmBranch { get; set; }

		[JsonProperty("unityVersion")]
		public string UnityVersion { get; set; }

		[JsonProperty("localUnityVersion")]
		public string LocalUnityVersion { get; set; }

		[JsonProperty("auditChanges")]
		public long AuditChanges { get; set; }

		[JsonProperty("projectVersion")]
		public ProjectVersion ProjectVersion { get; set; }

		[JsonProperty("projectName")]
		public string ProjectName { get; set; }

		[JsonProperty("projectId")]
		public string ProjectId { get; set; }

		[JsonProperty("projectGuid")]
		public Guid ProjectGuid { get; set; }

		[JsonProperty("orgId")]
		public string OrgId { get; set; }

		[JsonProperty("orgFk")]
		public string OrgFk { get; set; }

		[JsonProperty("filetoken")]
		public string Filetoken { get; set; }

		[JsonProperty("links")]
		public Links Links { get; set; }

		[JsonProperty("buildReport")]
		public BuildReport BuildReport { get; set; }
	}

	public partial class BuildReport
	{
		[JsonProperty("errors")]
		public long Errors { get; set; }

		[JsonProperty("warnings")]
		public long Warnings { get; set; }
	}

	public partial class Changeset
	{
		[JsonProperty("commitId")]
		public string CommitId { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("timestamp")]
		public DateTimeOffset Timestamp { get; set; }

		[JsonProperty("_id")]
		public string Id { get; set; }

		[JsonProperty("author")]
		public Author Author { get; set; }

		[JsonProperty("numAffectedFiles")]
		public long NumAffectedFiles { get; set; }
	}

	public partial class Author
	{
		[JsonProperty("fullName")]
		public string FullName { get; set; }

		[JsonProperty("absoluteUrl")]
		public string AbsoluteUrl { get; set; }
	}

	public partial class Links
	{
		[JsonProperty("self")]
		public Auditlog Self { get; set; }

		[JsonProperty("log")]
		public Auditlog Log { get; set; }

		[JsonProperty("auditlog")]
		public Auditlog Auditlog { get; set; }

		[JsonProperty("create_share")]
		public Auditlog CreateShare { get; set; }

		[JsonProperty("revoke_share")]
		public Auditlog RevokeShare { get; set; }

		[JsonProperty("icon")]
		public Auditlog Icon { get; set; }

		[JsonProperty("download_primary")]
		public DownloadPrimary DownloadPrimary { get; set; }

		[JsonProperty("artifacts")]
		public Artifact[] Artifacts { get; set; }
	}

	public partial class Artifact
	{
		[JsonProperty("key")]
		public string Key { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("primary")]
		public bool Primary { get; set; }

		[JsonProperty("show_download")]
		public bool ShowDownload { get; set; }

		[JsonProperty("files")]
		public File[] Files { get; set; }
	}

	public partial class File
	{
		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("size")]
		public long Size { get; set; }

		[JsonProperty("resumable")]
		public bool Resumable { get; set; }

		[JsonProperty("md5sum")]
		public string Md5Sum { get; set; }

		[JsonProperty("href")]
		public Uri Href { get; set; }
	}

	public partial class Auditlog
	{
		[JsonProperty("method")]
		public Method Method { get; set; }

		[JsonProperty("href")]
		public string Href { get; set; }
	}

	public partial class DownloadPrimary
	{
		[JsonProperty("method")]
		public Method Method { get; set; }

		[JsonProperty("href")]
		public Uri Href { get; set; }

		[JsonProperty("meta")]
		public Meta Meta { get; set; }
	}

	public partial class Meta
	{
		[JsonProperty("type")]
		public string Type { get; set; }
	}

	public partial class ProjectVersion
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("filename")]
		public string Filename { get; set; }

		[JsonProperty("projectName")]
		public string ProjectName { get; set; }

		[JsonProperty("platform")]
		public string Platform { get; set; }

		[JsonProperty("size")]
		public long Size { get; set; }

		[JsonProperty("created")]
		public DateTimeOffset Created { get; set; }

		[JsonProperty("lastMod")]
		public DateTimeOffset LastMod { get; set; }

		[JsonProperty("udids")]
		public object[] Udids { get; set; }
	}

	public enum Method { Delete, Get, Post };

	public partial class CloudBuild
	{
		public static CloudBuild[] FromJson(string json) =>
			JsonConvert.DeserializeObject<CloudBuild[]>(json,
				UnityCloudBuildAPI.Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this CloudBuild[] self) =>
			JsonConvert.SerializeObject(self, UnityCloudBuildAPI.Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters =
			{
				MethodConverter.Singleton,
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
			},
		};
	}

	internal class MethodConverter : JsonConverter
	{
		public override bool CanConvert(Type t) => t == typeof(Method) || t == typeof(Method?);

		public override object ReadJson(JsonReader reader, Type t, object existingValue,
			JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null) return null;
			var value = serializer.Deserialize<string>(reader);
			switch (value)
			{
				case "delete":
					return Method.Delete;
				case "get":
					return Method.Get;
				case "post":
					return Method.Post;
			}
			throw new Exception("Cannot unmarshal type Method");
		}

		public override void WriteJson(JsonWriter writer, object untypedValue,
			JsonSerializer serializer)
		{
			if (untypedValue == null)
			{
				serializer.Serialize(writer, null);
				return;
			}
			var value = (Method)untypedValue;
			switch (value)
			{
				case Method.Delete:
					serializer.Serialize(writer, "delete");
					return;
				case Method.Get:
					serializer.Serialize(writer, "get");
					return;
				case Method.Post:
					serializer.Serialize(writer, "post");
					return;
			}
			throw new Exception("Cannot marshal type Method");
		}

		public static readonly MethodConverter Singleton = new MethodConverter();
	}
}