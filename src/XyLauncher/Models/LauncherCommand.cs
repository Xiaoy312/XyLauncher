using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace XyLauncher.Models
{
	public class LauncherCommand
	{
		public string Name { get; set; }
		public string Command { get; set; }
		public virtual bool IsDowndrop => false;

		public static LauncherCommand Parse(IConfigurationSection config)
		{
			if (config.GetChildren() is var children && children.Count() != 1)
			{
				throw new FormatException("Invalid command syntax at: " + config.Path);
			}

			var item = children.FirstOrDefault();
			if (item.GetChildren() is var nested && nested.Any())
				return LauncherDropdownCommand.Parse(item);

			return new LauncherCommand()
			{
				Name = item.Key,
				Command = item.Value
			};
		}
	}
	public class LauncherDropdownCommand : LauncherCommand
	{
		public override bool IsDowndrop => true;
		public LauncherCommand[] DropdownCommands { get; set; }

		public static new LauncherDropdownCommand Parse(IConfigurationSection config)
		{
			var commands = config.GetChildren().Select(LauncherDropdownChildCommand.Parse).ToArray();

			return new LauncherDropdownCommand
			{
				Name = config.Key,
				Command = (commands.FirstOrDefault(x => x.IsDefault) ?? commands.FirstOrDefault())?.Command,
				DropdownCommands = commands,
			};
		}
	}
	public class LauncherDropdownChildCommand : LauncherCommand
	{
		public bool IsDefault { get; set; }

		public static new LauncherDropdownChildCommand Parse(IConfigurationSection config)
		{
			var command = new LauncherDropdownChildCommand();
			foreach (var children in config.GetChildren())
			{
				command[children.Key] = children.Value;
			}
			return command;
		}
		public string this[string index]
		{
			set
			{
				switch (index)
				{
					case "name": Name = value; return;
					case "command": Command = value; return;
					case "isDefault": IsDefault = bool.TryParse(value, out var isDefault) && isDefault; return;

					default:
						Name = index;
						Command = value;
						return;
				}
			}
		}
	}
}
