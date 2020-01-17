using System;
using System.Collections.Generic;
using System.Linq;
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
		private List<ProjectDirectory> _allProjectDirectories;

		[Reactive] public string SearchTerm { get; set; }

		public List<ProjectDirectory> FilteredProjectDirectories { [ObservableAsProperty] get; }

		public ReactiveCommand<string, List<ProjectDirectory>> SearchCommand { get; }

		public MainWindowViewModel()
		{
			SearchCommand = ReactiveCommand.CreateFromTask<string, List<ProjectDirectory>>(SearchAsync);
			SearchCommand.ThrownExceptions.Subscribe();
			SearchCommand.ToPropertyEx(this, x => x.FilteredProjectDirectories);

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
			if (_allProjectDirectories == null)
			{
				var config = ServiceProvider.GetService<AppConfig>();
				_allProjectDirectories = ProjectCrawler.Scan(config.RootDirectories).ToList();
			}

			return ProjectCrawler.Filter(_allProjectDirectories, searchTerm).ToList();
		}
	}
}
