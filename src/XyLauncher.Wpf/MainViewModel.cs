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

		public List<ProjectMatchResult> FilteredProjectDirectories { [ObservableAsProperty] get; }

		public IReadOnlyCollection<LauncherCommand> LauncherCommands { get; }

		public ReactiveCommand<string, List<ProjectMatchResult>> SearchCommand { get; }

		public ReactiveCommand<object[], Unit> ExecuteLauncherCommand { get; }

		public MainWindowViewModel()
		{
			var config = ServiceProvider.GetService<AppConfig>();
			_projectDirectories = ProjectCrawler.Scan(config.RootDirectories).ToArray();
			LauncherCommands = config.Commands;

			SearchCommand = ReactiveCommand.CreateFromTask<string, List<ProjectMatchResult>>(SearchAsync);
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

		public async Task<List<ProjectMatchResult>> SearchAsync(string searchTerm)
		{
			return ProjectCrawler.Match(_projectDirectories, searchTerm)
				.Where(x => x.Success)
				.OrderByDescending(x => x.MatchGroups.Any(y => y.Part == MatchGroupPart.Shortcut))
				.ThenByDescending(x => x.MatchGroups.Count(y => y.Part == MatchGroupPart.Shortcut) - x.Project.RootShortcut.Length)
				.ThenByDescending(x => x.MatchGroups.Count)
				.ToList();
		}

		public async Task LaunchAsync(object[] parameters)
		{
			if (parameters[0] is ProjectMatchResult item && parameters[1] is LauncherCommand command)
			{
				try
				{
					var fragments = command.Command.Split(' ', 2);
					var psi = new ProcessStartInfo(fragments[0], fragments[1].Replace("$0", item.Project.Path))
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
