using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XyLauncher.Helpers;
using XyLauncher.Models;

namespace XyLauncher
{
	public static class ProjectCrawler
	{
		public static IEnumerable<ProjectDirectory> Scan(IEnumerable<ScannableRootDirectory> rootDirectories)
		{
			return rootDirectories.SelectMany(x => new DirectoryInfo(x.Path).GetDirectories()
				.Where(y => !rootDirectories.Any(z => PathHelper.IsSame(z.Path, y.FullName)))
				.Select(y => new ProjectDirectory
				{
					Path = y.FullName,
					Name = y.Name,
					RootShortcut = x.Shortcut,
					RootDirectoryName = y.Parent.Name,
				})
			);
		}

		public static IEnumerable<ProjectDirectory> Filter(IEnumerable<ProjectDirectory> projects, string filter)
		{
			if (filter?.Split(' ') is string[] tokens && tokens?.Any() == true)
			{
				return projects
					.Where(x =>
						tokens.All(y => x.Name.Contains(y, StringComparison.InvariantCultureIgnoreCase)) ||
						(x.RootShortcut == tokens[0] && tokens.Skip(1).All(y => x.Name.Contains(y, StringComparison.InvariantCultureIgnoreCase)))
					)
					.OrderByDescending(x => x.RootShortcut == tokens[0]);
			}
			else
			{
				return projects;
			}
		}
	}
}