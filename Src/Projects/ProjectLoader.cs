namespace GodotEnvProjectManager;

using System;
using Godot;
using System.IO;
using System.Threading.Tasks;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using FileAccess = Godot.FileAccess;

public interface IProjectLoader : INode;

[Meta(typeof(IAutoNode))]
public partial class ProjectLoader : Node, IProjectLoader
{
	public override void _Notification(int what) => this.Notify(what);

	private const string SAVE_SECTION_NAME = "Prferences";
	private const string SAVE_FOLDER_PATH_KEY = "FolderPath";
	private const string SAVE_CONFIG_PATH = "user://config.cfg";
	private const string GODOT_PROJECT_TYPE = "GD";
	private const string DOTNET_PROJECT_TYPE = "C#";

	[Node("%ProjectsParent")] private IVBoxContainer ProjectParent { get; set; } = default!;
	[Node("%ChooseFolder")] private IButton ChooseFolderButton { get; set; } = default!;
	[Node("%FileDialog")] private IFileDialog FileDialog { get; set; } = default!;

	[Export] private PackedScene _projectInfos;

	public void OnResolved()
	{
		var config = new ConfigFile();

		var error = config.Load(SAVE_CONFIG_PATH);

		if (error == Error.Ok)
		{
			LoadFolder((string)config.GetValue(SAVE_SECTION_NAME, SAVE_FOLDER_PATH_KEY));
		}

		ChooseFolderButton.Pressed += FileDialog.Show;
		FileDialog.DirSelected += LoadFolder;
	}

	public void LoadFolder(string dir)
	{
		var config = new ConfigFile();
		config.SetValue(SAVE_SECTION_NAME, SAVE_FOLDER_PATH_KEY, dir);
		config.Save(SAVE_CONFIG_PATH);
		foreach (var gameDirectory in Directory.GetDirectories(dir))
		{
			foreach (var file in Directory.GetFiles(gameDirectory))
			{
				var fileName = Path.GetFileName(file);
				if (fileName == "project.godot")
				{
					LoadProject(file);
				}
			}
		}
	}

	private void LoadProject(string projectFilePath)
	{
		var projectConfig = new ConfigFile();
		projectConfig.Load(projectFilePath);
		var gameName = (string)projectConfig.GetValue("application", "config/name");
		var features = (string[])projectConfig.GetValue("application", "config/features");
		var projectIconInternal = (string)projectConfig.GetValue("application", "config/icon");
		var projectIconPath =
			Path.Combine(projectFilePath.Replace("project.godot", ""), projectIconInternal.Split("//")[1]);
		var projectType = GODOT_PROJECT_TYPE;
		if (features.Length > 2)
		{
			projectType = DOTNET_PROJECT_TYPE;
		}
		var newInformation = _projectInfos.Instantiate<ProjectInformation>();
		newInformation.SetValues(features[0], gameName, features[^1], projectType, projectFilePath, LoadProjectIcon(projectIconPath));
		ProjectParent.AddChild(newInformation);
	}

	private Texture2D LoadProjectIcon(string projectIconPath)
	{
		var iconFile = FileAccess.Open(projectIconPath, FileAccess.ModeFlags.Read);
		var bytes = iconFile.GetBuffer((long)iconFile.GetLength());
		var image = new Image();
		var data = image.LoadSvgFromBuffer(bytes);
		var imageTexture = new ImageTexture();
		imageTexture.SetImage(image);
		iconFile.Close();
		return imageTexture;
	}
}
