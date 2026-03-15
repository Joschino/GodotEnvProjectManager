using Godot;
using System.IO;
using System.Threading.Tasks;

public partial class ProjectLoader : Node
{
	const string DEFAULT_FOLDER = "D:\\Godot";

	[Export] VBoxContainer _projectParent;
	[Export] PackedScene _projectInfos;

	public override async void _Ready()
	{
		foreach (var gameDirectory in Directory.GetDirectories(DEFAULT_FOLDER))
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
			if (section.StartsWith("application"))
			{
				var parameters = section.Split("\n");
				var gameName = "DNLP";
				foreach (var parameter in parameters)
				{
					if (parameter.StartsWith("config/name"))
					{
						gameName = CleanString(parameter.Replace("config/name=", ""));
					}
					if (parameter.StartsWith("config/features=PackedStringArray"))
					{
						var features = parameter.Split('(')[1].Split(',');
						var newInformation = _projectInfos.Instantiate<ProjectInformation>();
						var projectType = features.Length > 2 ? features[1] : "GD";
						newInformation.Setup(CleanString(features[0]), gameName, CleanString(features[^1]),CleanString(projectType));
						_projectParent.AddChild(newInformation);
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
