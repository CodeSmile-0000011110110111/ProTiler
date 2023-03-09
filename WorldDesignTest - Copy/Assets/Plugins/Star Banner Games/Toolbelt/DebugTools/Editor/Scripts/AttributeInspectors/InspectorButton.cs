using System.Reflection;

namespace SBG.Toolbelt.DebugTools.Editor
{
	public struct InspectorButton
	{
		public string Name;
		public int Height;
		public MethodInfo Method;

		public InspectorButton(string name, int height, MethodInfo method)
        {
			Name = name;
			Height = height;
			Method = method;
        }
	}
}