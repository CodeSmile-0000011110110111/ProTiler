using UnityEngine;

namespace SBG.Toolbelt.DebugTools
{
	public class HelpBox : MonoBehaviour
	{
		#if UNITY_EDITOR
		public string HelpText = "";
        #endif
    }
}