using System;

namespace SBG.Toolbelt.AppLinker.Editor
{
	[Serializable]
	public class LinkedApp
	{
		public string Name;
		public string Path;
		public bool HasAssetMenu = true;

		[NonSerialized] public bool IsEditingName;

		public LinkedApp(string name, string path, bool hasAssetMenu=true)
		{
			Name = name;
			Path = path;
			HasAssetMenu = hasAssetMenu;
			IsEditingName = false;
		}
	}
}