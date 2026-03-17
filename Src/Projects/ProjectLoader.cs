namespace GodotEnvProjectManager;

using System;
using Godot;
using System.IO;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;

public interface IProjectLoader : INode;

[Meta(typeof(IAutoNode))]
public partial class ProjectLoader : Node, IProjectLoader
{
	public override void _Notification(int what) => this.Notify(what);

	private const string SECTION_NAME = "Prferences";
	private const string FOLDER_PATH_KEY = "FolderPath";
	private const string CONFIG_PATH = "user://config.cfg";

	[Node("%ProjectsParent")] private IVBoxContainer ProjectParent { get; set; } = default!;
	[Node("%ChooseFolder")] private IButton ChooseFolderButton { get; set; } = default!;
	[Node("%FileDialog")] private IFileDialog FileDialog { get; set; } = default!;

	[Export] private PackedScene _projectInfos;

	public void OnResolved()
	{
		var config = new ConfigFile();

		var error = config.Load(CONFIG_PATH);

		if (error == Error.Ok)
		{
			LoadFolder((string)config.GetValue(SECTION_NAME, FOLDER_PATH_KEY));
		}

		ChooseFolderButton.Pressed += FileDialog.Show;
		FileDialog.DirSelected += LoadFolder;
	}

	public void LoadFolder(string dir)
	{
		var config = new ConfigFile();
		config.SetValue(SECTION_NAME, FOLDER_PATH_KEY, dir);
		config.Save(CONFIG_PATH);
		foreach (var gameDirectory in Directory.GetDirectories(dir))
		{
			foreach (var file in Directory.GetFiles(gameDirectory))
			{
				var fileName = Path.GetFileName(file);
				if (fileName == "project.godot")
				{
					_ = LoadProject(file);
				}
			}
		}
	}

	private async Task LoadProject(string projectFilePath)
	{
		using StreamReader reader = new (projectFilePath);
		var fileText = await reader.ReadToEndAsync();

		var sections = fileText.Split('[');
		foreach (var section in sections)
		{
			if (section.StartsWith("application", StringComparison.InvariantCulture))
			{
				var parameters = section.Split("\n");
				var gameName = "DNLP";
				foreach (var parameter in parameters)
				{
					if (parameter.StartsWith("config/name", StringComparison.InvariantCulture))
					{
						gameName = CleanString(parameter.Replace("config/name=", ""));
					}
					if (parameter.StartsWith("config/features=PackedStringArray", StringComparison.InvariantCulture))
					{
						var features = parameter.Split('(')[1].Split(',');
						var newInformation = _projectInfos.Instantiate<ProjectInformation>();
						var projectType = features.Length > 2 ? features[1] : "GD";
						newInformation.SetValues(CleanString(features[0]), gameName, CleanString(features[^1]),CleanString(projectType), projectFilePath);
						ProjectParent.AddChild(newInformation);
						break;
					}
				}
			}
		}
	}

	private string CleanString(string stringToClean)
	{
		return stringToClean.Replace("\"", "").Replace(")", "");
	}
}
