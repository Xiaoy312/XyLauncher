using System;
using System.Collections.Generic;
using System.Text;

namespace XyLauncher.Models
{
	public partial class MatchGroup
	{
		public MatchGroupPart Part { get; set; }

		public int Index { get; set; }

		public int Length { get; set; }

		public string Value { get; set; }
	}
}
