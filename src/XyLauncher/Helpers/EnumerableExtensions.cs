using System;
using System.Collections.Generic;
using System.Text;

namespace XyLauncher.Helpers
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<IEnumerable<T>> GroupContinuously<T>(this IEnumerable<T> source, Func<T, T, bool> areContinous)
		{
			var buffer = new List<T>();
			var etor = source.GetEnumerator();
			if (!etor.MoveNext()) yield break;

			var last = etor.Current;
			buffer.Add(last);

			while (etor.MoveNext())
			{
				if (!areContinous(last, etor.Current))
				{
					yield return buffer;
					buffer.Clear();
				}

				last = etor.Current;
				buffer.Add(last);
			}

			yield return buffer;
		}
	}
}
