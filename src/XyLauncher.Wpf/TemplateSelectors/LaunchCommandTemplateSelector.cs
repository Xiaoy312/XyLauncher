using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using XyLauncher.Models;

namespace XyLauncher.Wpf.TemplateSelectors
{
	public class LaunchCommandTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DefaultLauncherCommandTemplate { get; set; }

		public DataTemplate LauncherDropdownCommandTemplate { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is LauncherDropdownCommand downdrop)
			{
				return LauncherDropdownCommandTemplate;
			}
			else if (item is LauncherCommand)
			{
				return DefaultLauncherCommandTemplate;
			}
			else
			{
				return default;
			}
		}
	}
}
