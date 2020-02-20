using System;
using System.Collections.Generic;
using System.Text;

namespace XyLauncher.Models
{
	public class ProjectMatchResult
	{
		public ProjectDirectory Project { get; set; }

		public ICollection<MatchGroup> MatchGroups { get; set; } = new List<MatchGroup>();

		public bool Success { get; set; }
	}
}
