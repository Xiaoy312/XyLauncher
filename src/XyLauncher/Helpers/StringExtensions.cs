using System;
using System.Collections.Generic;
using System.Text;

namespace XyLauncher.Helpers
{
	internal static class StringExtensions
	{
		public static bool Contains(this string text, string value, StringComparison comparison)
		{
			return text?.IndexOf(value, comparison) >= 0;
		}
	}
}
