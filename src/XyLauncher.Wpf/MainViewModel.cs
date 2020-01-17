using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using XyLauncher.Models;

namespace XyLauncher.Wpf
{
	public class MainWindowViewModel : ReactiveObject
	{
		private IReadOnlyCollection<ProjectDirectory> _projectDirectories;

		[Reactive] public string SearchTerm { get; set; }

		public List<ProjectDirectory> FilteredProjectDirectories { [ObservableAsProperty] get; }

		public IReadOnlyCollection<LauncherCommand> LauncherCommands { get; }

		public ReactiveCommand<string, List<ProjectDirectory>> SearchCommand { get; }

		public ReactiveCommand<object[], Unit> ExecuteLauncherCommand { get; }

		public MainWindowViewModel()
		{
			var config = ServiceProvider.GetService<AppConfig>();
			_projectDirectories = ProjectCrawler.Scan(config.RootDirectories).ToArray();
			LauncherCommands = config.Commands;

			SearchCommand = ReactiveCommand.CreateFromTask<string, List<ProjectDirectory>>(SearchAsync);
			SearchCommand.ThrownExceptions.Subscribe();
			SearchCommand.ToPropertyEx(this, x => x.FilteredProjectDirectories);

			ExecuteLauncherCommand = ReactiveCommand.CreateFromTask<object[]>(LaunchAsync);

			ExecuteLauncherCommand.ThrownExceptions.Subscribe();

			this.WhenAnyValue(x => x.SearchTerm)
				.Throttle(TimeSpan.FromSeconds(0.25))
				.InvokeCommand(SearchCommand);
		}

		private static IServiceProvider ServiceProvider { get; } = new ServiceCollection()
			.AddSingleton(LoadConfig)
			.AddSingleton<AppConfig>()
			.BuildServiceProvider();

		private static IConfiguration LoadConfig(IServiceProvider provider) => new ConfigurationBuilder()
			.AddYamlFile("XyLauncher.yml", false, true)
			.Build();

		public async Task<List<ProjectDirectory>> SearchAsync(string searchTerm)
		{
			return ProjectCrawler.Filter(_projectDirectories, searchTerm).ToList();
		}

		public async Task LaunchAsync(object[] parameters)
		{
			if (parameters[0] is ProjectDirectory project && parameters[1] is LauncherCommand command)
			{
				try
				{
					var fragments = command.Command.Split(' ', 2);
					var psi = new ProcessStartInfo(fragments[0], fragments[1].Replace("$0", project.Path))
					{
						UseShellExecute = true
					};

					Process.Start(psi);
				}
				catch (Exception e)
				{
				}
			}
		}
	}
}
