namespace GodotEnvProjectManager;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IVersionInformation : IPanel;

[Meta(typeof(IAutoNode))]
public partial class VersionInformation : Panel, IVersionInformation
{
	public override void _Notification(int what) => this.Notify(what);

	[Node("%VersionName")] private ILabel _versionNameLabel { get; set; } = default!;
	[Node("%IsInstalled")] private ILabel _isInstalledLabel { get; set; } = default!;
	[Node("%IsMonoInstalled")] private ILabel _isMonoInstalledLabel { get; set; } = default!;

	private string _versionName;
	private bool _isInstalled;
	private bool _isMonoInstalled;

	public void OnResolved()
	{
		_versionNameLabel.Text = _versionName;
		_isInstalledLabel.Text = $"Is Installed: {_isInstalled}";
		_isMonoInstalledLabel.Text = $"Is Mono Installed: {_isMonoInstalled}";
	}

	public void SetValues(string versionName, bool isInstalled, bool isMonoInstalled)
	{
		_versionName = versionName;
		_isInstalled = isInstalled;
		_isMonoInstalled = isMonoInstalled;
	}
}
