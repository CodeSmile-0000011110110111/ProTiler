// Copyright (C) 2021-2023 Steffen Itterheim
// Refer to included LICENSE file for terms and conditions.

using UnityEditor.Toolbars;
using UnityEngine.UIElements;

namespace CodeSmile.Editor.ProTiler.Overlays
{
	internal class TileLayerToolbarRadioToggle : EditorToolbarToggle
	{
		internal static void TurnAllToolbarTogglesOff(VisualElement toggle)
		{
			foreach (var element in toggle.parent.Children())
			{
				var otherToggle = element as TileLayerToolbarRadioToggle;
				if (otherToggle != null && otherToggle != toggle)
					otherToggle.SetValueWithoutNotify(false);
			}
		}

		public TileLayerToolbarRadioToggle()
		{
			this.RegisterValueChangedCallback(OnRadioToggleChange);
		}

		private void OnRadioToggleChange(ChangeEvent<bool> evt)
		{
			if (evt.newValue)
				TurnAllToolbarTogglesOff(this);
			else
				SetValueWithoutNotify(true);
		}
	}
}