using System.IO;
using System.Linq;
using NUnit.Framework;
using XyLauncher.Models;

namespace XyLauncher.Tests
{
	public class MatchingTests
	{
		[Test]
		public void ContinuousLettersMatch_Should_RankHigher()
		{
			var query = "asd";
			var projects = @"AppleSauceDriver,AsdSomething".Split(',')
				.Select(x => new ProjectDirectory($@"D:\code\lib\{x}", x, "c", "c#"))
				.OrderBy(x => x.RootDirectoryName);

			var results = ProjectCrawler.RankedMatch(projects, query);
			var topResult = results.FirstOrDefault();

			Assert.AreEqual(topResult.Project.Name, "AsdSomething");
		}

		[TestCase("p", "projects")]
		[TestCase("pl", "platform")]
		public void FullShortcutMatch_Should_RankHigher(string query, string expected)
		{
			var projects = new (string Shortcut, string RootDirectory, string[] projects)[]
			{
				("p", "projects", "L.Something".Split(',')),
				("pl", "platform", "DontCare".Split(',')),
			}
				.SelectMany(x => x.projects
					// implicitly sorted by RootDirectory first, and then by project name (y)
					//.OrderBy(y => y)
					.Select(y => new ProjectDirectory(Path.Combine(@"D:\code\uno", x.RootDirectory, y), y, x.Shortcut, x.RootDirectory))
				);

			var results = ProjectCrawler.RankedMatch(projects, query);
			var topResult = results.FirstOrDefault();

			Assert.AreEqual(topResult.Project.RootDirectoryName, expected);
		}
	}
}