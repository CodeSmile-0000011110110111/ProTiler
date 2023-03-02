using UnityEngine;
using UnityEngine.UI;

namespace SBG.Toolbelt.UI
{
	[System.Serializable]
	public class BlackScreen
	{
		[SerializeField] private Image panel;

		public void Init()
		{
			Disable();
		}

        public void ShowBlackscreen(Color screenColor)
        {
            panel.color = screenColor;
            panel.raycastTarget = true;
            panel.enabled = true;
        }

        public void Disable()
        {
            if (!panel.enabled) return;

            panel.color = Color.clear;
            panel.raycastTarget = false;
            panel.enabled = false;
        }
    }
}