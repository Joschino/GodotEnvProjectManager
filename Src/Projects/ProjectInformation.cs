namespace GodotEnvProjectManager;

using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IProjectInformation : IPanel;

[Meta(typeof(IAutoNode))]
public partial class ProjectInformation : Panel, IProjectInformation
{
	public override void _Notification(int what) => this.Notify(what);

	[Node("%GameName")] ILabel _gameNameLabel { get; set; } = default;
	[Node("%GodotVersion")] ILabel _godotVersionLabel { get; set; } = default;
	[Node("%Renderer")] ILabel _rendererLabel { get; set; } = default;
	[Node("%ProjectType")] ILabel _projectTypeLabel { get; set; } = default;

	private string _godotVersion = string.Empty;
	private string _gameName = string.Empty;
	private string _renderer = string.Empty;
	private string _projectType = string.Empty;

	public void OnResolved()
	{
		_gameNameLabel.SetText(_gameName);
		_godotVersionLabel.SetText(_godotVersion);
		_rendererLabel.SetText(_renderer);
		_projectTypeLabel.SetText(_projectType);
	}

	public void SetValues(string version, string gameName, string renderer, string projectType)
	{
		_godotVersion = version;
		_gameName = gameName;
		_renderer = renderer;
		_projectType = projectType;
	}


}
