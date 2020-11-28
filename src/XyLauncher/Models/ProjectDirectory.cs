namespace XyLauncher.Models
{
	public class ProjectDirectory
	{
		public string Path { get; set; }

		public string Name { get; set; }

		public string RootShortcut { get; set; }

		public string RootDirectoryName { get; set; }

		public ProjectDirectory()
		{
		}

		public ProjectDirectory(string path, string name, string rootShortcut, string rootDirectoryName)
		{
			this.Path = path;
			this.Name = name;
			this.RootShortcut = rootShortcut;
			this.RootDirectoryName = rootDirectoryName;
		}
	}
}