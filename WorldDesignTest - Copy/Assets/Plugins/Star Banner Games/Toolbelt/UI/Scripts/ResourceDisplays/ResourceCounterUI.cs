using TMPro;
using UnityEngine;

namespace SBG.Toolbelt.UI
{
	/// <summary>
	/// Use this Behaviour to create simple Resource Displays
	/// </summary>
	public class ResourceCounterUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI displayText;
		[SerializeField] private float maxValue = 100;
		[Space]
		public ResourceDisplayMode DisplayMode = ResourceDisplayMode.Value;
		public string PrefixText;
		[Space]
		[Header("Value Change Effects")]
		[SerializeField] private float normalizeDuration = 0.25f;
		[SerializeField] private bool doBounce = false;
		[SerializeField] private float bounceScale = 1.2f;
		[SerializeField] private bool flashColor = false;
		[SerializeField] private Color increaseTint = Color.green;
		[SerializeField] private Color decreaseTint = Color.red;

		private Transform _textTransform;
		private Color _defaultColor;
		private Color _lerpColor;

		private float _lerpStart;
		private float _lastValue;
		private bool _isLerping = false;


        private void Awake()
        {
			_textTransform = displayText.transform;
			_defaultColor = displayText.color;
        }

        private void Update()
        {
            if (_isLerping)
            {
				float time = (Time.time - _lerpStart) / normalizeDuration;
				if (doBounce) _textTransform.localScale = Vector3.one * Mathf.Lerp(bounceScale, 1, time);
				if (flashColor) displayText.color = Color.Lerp(_lerpColor, _defaultColor, time);
            }
        }

		public void SetMaxValue(float max)
        {
			maxValue = max;
			SetValue(_lastValue, true);
        }

        public void SetValue(float value, bool skipAnimations=false)
		{
			value = Mathf.Clamp(value, 0, maxValue);

			//Set Prefix Text
			string displayString = "";

			if (PrefixText != string.Empty)
			{
				displayString = $"{PrefixText}: ";
			}

			//Display fitting Text String
			switch (DisplayMode)
			{
				case ResourceDisplayMode.Percentage:
					displayString += $"{value / maxValue * 100}%";
					break;
				case ResourceDisplayMode.IntPercentage:
					displayString += $"{value / maxValue * 100:0}%";
					break;
				case ResourceDisplayMode.Value:
					displayString += value;
					break;
				case ResourceDisplayMode.IntValue:
					displayString += $"{value:0}";
					break;
				default:
					displayString = value.ToString();
					break;
			}

			displayText.text = displayString;

			//If no Animations are needed, end here
			if (!doBounce && !flashColor || value == _lastValue || skipAnimations)
			{ 
				_lastValue = value;
				return; 
			}

			//Change Color based on increase/decrease
			if (value > _lastValue)
            {
				_lerpColor = increaseTint;
            }
            else
            {
				_lerpColor = decreaseTint;
			}

			//upscale text for bounce
			if (doBounce) _textTransform.localScale = Vector3.one * bounceScale;

			//Init Lerp
			_lerpStart = Time.time;
			_isLerping = true;

			_lastValue = value;
		}
	}

	public enum ResourceDisplayMode
    {
		Percentage,
		IntPercentage,
		Value,
		IntValue,
    }
}