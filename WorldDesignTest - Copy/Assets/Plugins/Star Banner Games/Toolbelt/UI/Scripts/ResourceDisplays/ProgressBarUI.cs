using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SBG.Toolbelt.UI
{
	/// <summary>
	/// Use this class to quickly create Logic for your UI Bars.
	/// You can add it as a SerializedField to configure it directly in the inspector.
	/// Look at the "ResourceBarUI" class to see an implementation of this
	/// </summary>
	[System.Serializable]
	public class ProgressBarUI
	{
		/// <summary>
		/// The ProgressBar will be at 100% fill at this value. Any higher values are clamped
		/// </summary>
		public float MaxValue = 100;
		public Image FillImage;
		[Header("Optional")]
		public TextMeshProUGUI DisplayText;

		public float Value { get; private set; }
		public float FillAmount { get; private set; }


		public ProgressBarUI(float maxValue, Image fillImage, TextMeshProUGUI displayText=null)
        {
			MaxValue = maxValue;
			FillImage = fillImage;
			DisplayText = displayText;
        }

		/// <summary>
		/// Updates the Fill as well as the Display Text of the Progress Bar
		/// </summary>
		/// <param name="displayMode">Display Mode of the Text</param>
		/// <param name="prefixText">Prefix before the Value Display (Shown as: "Prefix: Value")</param>
		public void SetValue(float value, ProgressBarTextMode displayMode = ProgressBarTextMode.Hidden, string prefixText = "")
		{
			float fillAmount = value / MaxValue;

			FillImage.fillAmount = fillAmount;

			Value = value;
			FillAmount = fillAmount;

			OverwriteText(value, displayMode, prefixText);
		}
	
		/// <summary>
		/// Overwrite the Display Text of the Progress Bar (if present).
		/// </summary>
		public void OverwriteText(float overwriteValue, ProgressBarTextMode displayMode, string prefixText = "")
        {
			if (DisplayText == null || displayMode == ProgressBarTextMode.KeepLastText)
			{
				return;
			}
			else if (displayMode == ProgressBarTextMode.Hidden)
			{
				if (!string.IsNullOrEmpty(DisplayText.text)) DisplayText.text = string.Empty;
				return;
			}

			//Add Prefix Text
			string displayString = "";

			if (prefixText != string.Empty)
			{
				displayString = $"{prefixText}: ";
			}

			//Recalculate Fill Amount (In case overwriteValue != Value)
			float fillAmount = overwriteValue / MaxValue;

			//Get fitting display string
			switch (displayMode)
			{
				case ProgressBarTextMode.Percentage:
					displayString += $"{fillAmount * 100}%";
					break;
				case ProgressBarTextMode.IntPercentage:
					displayString += $"{fillAmount * 100:0}%";
					break;
				case ProgressBarTextMode.Value:
					displayString += overwriteValue;
					break;
				case ProgressBarTextMode.IntValue:
					displayString += $"{overwriteValue:0}";
					break;
				default:
					displayString = "";
					break;
			}

			DisplayText.text = displayString;
		}
	}

	public enum ProgressBarTextMode
	{
		Hidden,
		KeepLastText,
		Percentage,
		IntPercentage,
		Value,
		IntValue,
	}
}