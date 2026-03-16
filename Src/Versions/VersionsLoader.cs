namespace GodotEnvProjectManager;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IVersionsLoader : INode;


[Meta(typeof(IAutoNode))]
public partial class VersionsLoader : Node, IVersionsLoader
{
	public override void _Notification(int what) => this.Notify(what);

	[Node("%VersionsParent")] private IVBoxContainer VersionsParent { get; set; } = default!;
	[Export] private PackedScene _versionInformation;

	private Dictionary<string, GodotVersionInformation> _versions = new();

	public void OnResolved()
	{
		LoadInstalledVersions();
		LoadAllVersions();

		foreach (var version in _versions.Values)
		{
			var newVersionInformation = _versionInformation.Instantiate<VersionInformation>();
			newVersionInformation.SetValues(version.VersionName, version.IsInstalled, version.IsMonoInstalled);
			VersionsParent.AddChild(newVersionInformation);
		}
	}

	private void LoadAllVersions()
	{
		var process = new Process
		{
			StartInfo =  new ProcessStartInfo
			{
				FileName = "godotenv",
				Arguments = "godot list -r",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};

		process.Start();
		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		foreach (var version in output.Split("\n").Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith("Retrieving", StringComparison.InvariantCulture)))
		{
			AddVersion(version);
		}
	}

	private void LoadInstalledVersions()
	{
		var process = new Process
		{
			StartInfo =  new ProcessStartInfo
			{
				FileName = "godotenv",
				Arguments = "godot list",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};

		process.Start();
		var output = process.StandardOutput.ReadToEnd();
		process.WaitForExit();

		foreach (var version in output.Split("\n").Where(x => !string.IsNullOrEmpty(x)))
		{
			var isMonoInstallation = version.Contains('*', StringComparison.InvariantCulture);
			AddVersion(version, !isMonoInstallation, isMonoInstallation);
		}
	}

	private void AddVersion(string version, bool isInstalled = false,  bool isMonoInstalled = false)
	{
		var cleanVersion = CleanVersionString(version);
		if (_versions.TryGetValue(cleanVersion, out var versionInformation))
		{
			if (!versionInformation.IsInstalled)
			{
				versionInformation.IsInstalled = isInstalled;
			}

			if (!versionInformation.IsMonoInstalled)
			{
				versionInformation.IsMonoInstalled = isMonoInstalled;
			}
		}
		else
		{
			var split = cleanVersion.Split('-');
			var newVersionInformation =
				new GodotVersionInformation(
					cleanVersion,
					split[1].Contains("stable", StringComparison.InvariantCulture),
					split[1].Contains("rc", StringComparison.InvariantCulture),
					split[1].Contains("dev", StringComparison.InvariantCulture),
					split[1].Contains("beta", StringComparison.InvariantCulture),
					split[1].Contains("alpha", StringComparison.InvariantCulture),
					isInstalled,
					isMonoInstalled);
			_versions.Add(version, newVersionInformation);
		}
	}

	private string CleanVersionString(string version)
	{
		return version.Split(' ')[0].Trim();
	}
}
