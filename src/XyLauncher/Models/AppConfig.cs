using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace XyLauncher.Models
{
	public class AppConfig
	{
		public IReadOnlyCollection<ScannableRootDirectory> RootDirectories { get; }

		public AppConfig(IConfiguration config)
		{
			RootDirectories = config.GetSection("root-directories").GetChildren()
				.Select(ScannableRootDirectory.Parse)
				.ToArray();
		}
	}

	public class ScannableRootDirectory
	{
		public string Path { get; set; }
		public string Shortcut { get; set; }

		public static ScannableRootDirectory Parse(IConfigurationSection config)
		{
			return new ScannableRootDirectory
			{
				Path = config["path"],
				Shortcut = config["shortcut"],
			};
		}
	}
}
