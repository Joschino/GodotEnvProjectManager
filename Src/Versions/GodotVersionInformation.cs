namespace GodotEnvProjectManager;

public record GodotVersionInformation(
	string versionName,
	bool isStableVersion = false,
	bool isReleaseClientVersion = false,
	bool isDevVersion = false,
	bool isBetaVersion = false,
	bool isAlphaVersion = false,
	bool isInstalled = false,
	bool isMonoInstalled = false )
{
	public string VersionName = versionName;
	public bool IsInstalled = isInstalled;
	public bool IsMonoInstalled = isMonoInstalled;
	public bool IsStableVersion = isStableVersion;
	public bool IsReleaseClientVersion = isReleaseClientVersion;
	public bool IsDevVersion = isDevVersion;
	public bool IsBetaVersion = isBetaVersion;
	public bool IsAlphaVersion = isAlphaVersion;
}
