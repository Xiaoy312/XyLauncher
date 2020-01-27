using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace XyLauncher.Wpf.Controls
{
	public class BindingProxy : Freezable
	{
		protected override Freezable CreateInstanceCore() => new BindingProxy();

		#region Property: Data
		public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
			nameof(Data),
			typeof(object),
			typeof(BindingProxy),
			new UIPropertyMetadata(default(object)));
		public object Data
		{
			get => (object)GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}
		#endregion
	}
}
