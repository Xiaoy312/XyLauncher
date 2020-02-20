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

		public static IEnumerable<ProjectMatchResult> Match(IEnumerable<ProjectDirectory> projects, string query) => projects?.Select(x => Match(x, query));

		public static ProjectMatchResult Match(ProjectDirectory project, string query)
		{
			if (string.IsNullOrEmpty(query)) return new ProjectMatchResult { Project = project, Success = true };

			var matches = new List<MatchGroup>();
			var part = MatchGroupPart.Shortcut;
			var partIndex = 0;
			var success = true;

			foreach (var c in query.ToLowerInvariant())
			{
				if (part == MatchGroupPart.Shortcut)
				{
					if (SearchFromIndex(project.RootShortcut, c, partIndex) is var index && index != -1)
					{
						matches.Add(new MatchGroup { Part = part, Index = index, Value = project.RootShortcut.Substring(index, 1) });
						partIndex = index + 1;

						if (project.RootShortcut.Length >= partIndex) continue;
					}
					if (c == ' ') continue;

					part = MatchGroupPart.ProjectName;
					partIndex = 0;
				}
				if (part == MatchGroupPart.ProjectName)
				{
					if (SearchFromIndex(project.Name, c, partIndex) is var index && index != -1)
					{
						matches.Add(new MatchGroup { Part = part, Index = index, Value = project.Name.Substring(index, 1) });
						partIndex = index + 1;
						continue;
					}
					if (c == ' ') continue;

					success = false;
					break;
				}
			}

			return new ProjectMatchResult
			{
				Project = project,
				MatchGroups = matches
					.GroupContinuously((a, b) => a.Part == b.Part && (a.Index + 1) == b.Index)
					.Select(g => new MatchGroup
					{
						Part = g.First().Part,
						Index = g.First().Index,
						Length = g.Count(),
					})
					.ToList(),
				Success = success
			};

			int SearchFromIndex(string text, char c, int index) => text.ToLowerInvariant().IndexOf(c, index);
		}
	}
}