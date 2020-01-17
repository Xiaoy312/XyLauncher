using System;
using System.IO;

namespace XyLauncher.Helpers
{
	public static class PathHelper
	{
		public static string NormalizeDirectorySeparator(string path)
		{
			return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		}

		public static bool IsSame(string path0, string path1, bool ignoreTrailingSeparator = true)
		{
			var normalize = ignoreTrailingSeparator
				? x => NormalizeDirectorySeparator(x).TrimEnd(Path.DirectorySeparatorChar)
				: (Func<string, string>)NormalizeDirectorySeparator;

			return normalize(path0) == normalize(path1);
		}
	}
}