using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using XyLauncher.Models;

namespace XyLauncher.Wpf.Behaviors
{
	public static class ProjectMatchResultHightlightBehavior
	{
		#region Property: Context
		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached(
			"Context",
			typeof(ProjectMatchResult),
			typeof(ProjectMatchResultHightlightBehavior),
			new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.AffectsRender, ContextChanged)
		);

		public static ProjectMatchResult GetContext(TextBlock obj) => (ProjectMatchResult)obj.GetValue(ContextProperty);
		public static void SetContext(TextBlock obj, ProjectMatchResult value) => obj.SetValue(ContextProperty, value);
		#endregion

		private static void ContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBlock textblock && e.NewValue is ProjectMatchResult result)
			{
				UpdateTextBlockContent(textblock, result);
			}
		}

		private static void UpdateTextBlockContent(TextBlock textblock, ProjectMatchResult result)
		{
			textblock.Inlines.Clear();

			textblock.Inlines.AddRange(SliceTransform(
				$"[{result.Project.RootShortcut}] ",
				result.MatchGroups.Where(x => x.Part == MatchGroupPart.Shortcut),
				x => (x.Index, x.Length),
				(cut, value) => new Run
				{
					Text = value,
					Foreground = (cut != null && textblock.FindResource("MahApps.Brushes.Accent") is Brush accent)
						? accent
						: textblock.Foreground,
				},
				offset: 1 // for "["
			));
			textblock.Inlines.AddRange(SliceTransform(
				result.Project.Name,
				result.MatchGroups.Where(x => x.Part == MatchGroupPart.ProjectName),
				x => (x.Index, x.Length),
				(cut, value) => new Run
				{
					Text = value,
					FontWeight = FontWeights.Bold,
					Foreground = (cut != null && textblock.FindResource("MahApps.Brushes.Accent") is Brush accent)
						? accent
						: textblock.Foreground,
				}
			));
		}

		public static IEnumerable<TResult> SliceTransform<TCut, TResult>(string text, IEnumerable<TCut> cuts, Func<TCut, (int Index, int Length)> sliceSelector, Func<TCut, string, TResult> resultSelector, int offset = 0)
		{
			if (cuts?.Any() != true)
			{
				yield return resultSelector(default, text);
				yield break;
			}
			if (text == null) yield break;

			var index = 0;
			foreach (var cut in cuts)
			{
				var slice = sliceSelector(cut);
				slice = (slice.Index + offset, slice.Length);

				if (index > slice.Index) throw new ArgumentOutOfRangeException();
				if (index < slice.Index)
				{
					yield return resultSelector(default, text.Substring(index, slice.Index - index));
					index = slice.Index + slice.Length;
				}

				yield return resultSelector(cut, text.Substring(slice.Index, slice.Length));
				index = slice.Index + slice.Length;
			}

			if (index < text.Length) yield return resultSelector(default, text.Substring(index));
		}
	}
}
