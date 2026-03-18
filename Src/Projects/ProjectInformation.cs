namespace GodotEnvProjectManager;

using System.Diagnostics;
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
	[Node("%ProjectIcon")] ITextureRect _projectIconTextureRect { get; set; } = default;

	private string _godotVersion = string.Empty;
	private string _gameName = string.Empty;
	private string _renderer = string.Empty;
	private string _projectType = string.Empty;
	private string _fullPath = string.Empty;
	private Texture2D _projectIcon;

	public void OnResolved()
	{
		_gameNameLabel.SetText(_gameName);
		_godotVersionLabel.SetText(_godotVersion);
		_rendererLabel.SetText(_renderer);
		_projectTypeLabel.SetText(_projectType);
		_projectIconTextureRect.Texture = _projectIcon;
	}

	public void SetValues(string version, string gameName, string renderer, string projectType, string fullPath, Texture2D projectIcon)
	{
		_godotVersion = version;
		_gameName = gameName;
		_renderer = renderer;
		_projectType = projectType;
		_fullPath = fullPath;
		_projectIcon = projectIcon;
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton { DoubleClick: true })
		{
			StartEditor();
		}
	}

	private void StartEditor()
	{
		var process = new Process
		{
			StartInfo =  new ProcessStartInfo
			{
				FileName = "godot",
				Arguments = $"\"{_fullPath}\" -e",
				RedirectStandardOutput = false,
				RedirectStandardError = false,
				UseShellExecute = true,
				CreateNoWindow = true
			}
		};

		process.Start();
	}
}
