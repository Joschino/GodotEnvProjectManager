namespace GodotEnvProjectManager;

using System.Diagnostics;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;

public interface IVersionInformation : IPanel;

[Meta(typeof(IAutoNode))]
public partial class VersionInformation : Panel, IVersionInformation
{
	public override void _Notification(int what) => this.Notify(what);

	[Node("%VersionName")] private ILabel VersionNameLabel { get; set; } = default!;
	[Node("%IsInstalled")] private ILabel IsInstalledLabel { get; set; } = default!;
	[Node("%IsMonoInstalled")] private ILabel IsMonoInstalledLabel { get; set; } = default!;
	[Node("%PopupMenu")] private IPopupMenu PopupMenu { get; set; } = default!;

	private string _versionName;
	private bool _isInstalled;
	private bool _isMonoInstalled;

	public void OnResolved()
	{
		VersionNameLabel.Text = _versionName;
		IsInstalledLabel.Text = $"Is Installed: {_isInstalled}";
		IsMonoInstalledLabel.Text = $"Is Mono Installed: {_isMonoInstalled}";
		PopupMenu.AddItem("Install Dotnet", 0);
		PopupMenu.AddItem("Install Default", 1);
		PopupMenu.AddItem("Uninstall All", 2);
		PopupMenu.IdPressed += PopupMenuOnIdPressed;
	}

	private void PopupMenuOnIdPressed(long id)
	{
		switch (id)
		{
			case 0:
				InstallDotnet();
				break;
			case 1:
				InstallDefault();
				break;
		}
	}

	private void UninstallAll()
	{
		throw new System.NotImplementedException();
	}

	private void InstallDefault()
	{
		var process = new Process
		{
			StartInfo =  new ProcessStartInfo
			{
				FileName = "godotenv",
				Arguments = $"godot install {_versionName} --no-dotnet",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};

		process.Start();
		process.WaitForExit();

		GD.Print($"Should have installed version for {_versionName}");
	}

	private void InstallDotnet()
	{
		var process = new Process
		{
			StartInfo =  new ProcessStartInfo
			{
				FileName = "godotenv",
				Arguments = $"godot install {_versionName}",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			}
		};

		process.Start();
		process.WaitForExit();

		GD.Print($"Should have installed dotnet version for {_versionName}");
	}

	public void SetValues(string versionName, bool isInstalled, bool isMonoInstalled)
	{
		_versionName = versionName;
		_isInstalled = isInstalled;
		_isMonoInstalled = isMonoInstalled;
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right)
		{
			PopupMenu.Show();
		}
	}
}
