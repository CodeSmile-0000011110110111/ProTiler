using UnityEngine;
using UnityEngine.UI;

namespace SBG.Toolbelt.UI
{
	/// <summary>
	/// An implementation of a ResourceBar. Use this to quickly implement Health/Resource Bars
	/// </summary>
	public class ResourceBarUI : MonoBehaviour
	{
		[SerializeField] private ProgressBarUI ProgressBar;
		public ProgressBarTextMode TextDisplayMode = ProgressBarTextMode.Hidden;
		public ResourceTextUpdateMode TextUpdateMode = ResourceTextUpdateMode.Lerp;
		public string PrefixText;
		[Space]
		[Range(0f, 2f)]
		[SerializeField] private float fillDelay = 0.5f;
		[Range(0.1f, 10f)]
		[SerializeField] private float fillPerSecond = 3f;

		[Header("Optional")]
		[SerializeField] private Image previewFillImage;
		[SerializeField] private Color increaseTint = Color.green;
		[SerializeField] private Color decreaseTint = Color.red;

		private bool _isLerping = false;
		private float _lerpTime;
		private float _lerpStartValue;
		private float _lerpTargetValue;
		private bool _lerpIsGain;
		private float _currentLerpValue;


		public void SetMaxValue(float newMax)
		{
			ProgressBar.MaxValue = newMax;
		}

		public void SetValue(float newValue)
        {
			if (newValue == ProgressBar.Value && !_isLerping) return;

			//If there is no Preview Bar, just set the Value instantly
			if (previewFillImage == null)
            {
				ProgressBar.SetValue(newValue, TextDisplayMode, PrefixText);
				return;
            }

			//Clamp Value
			newValue = Mathf.Clamp(newValue, 0, ProgressBar.MaxValue);

			//Init Lerp
			_lerpStartValue = ProgressBar.Value;
			bool isGain = newValue > ProgressBar.Value; //Is there a Value increase or decrease?

			previewFillImage.color = isGain ? increaseTint : decreaseTint;

			if (isGain)
			{
				//Set Preview Fill
				previewFillImage.fillAmount = newValue / ProgressBar.MaxValue;

				//Update Display Text
				if (TextUpdateMode == ResourceTextUpdateMode.Instant)
					ProgressBar.OverwriteText(newValue, TextDisplayMode, PrefixText);
			}
            else
            {
				//Set Preview Fill
				if (!_isLerping) previewFillImage.fillAmount = ProgressBar.FillAmount;
				else _lerpStartValue = _currentLerpValue;

				//Update Display Text
				ProgressBarTextMode mode = TextUpdateMode == ResourceTextUpdateMode.Instant ? TextDisplayMode : ProgressBarTextMode.KeepLastText;
				ProgressBar.SetValue(newValue, mode, PrefixText);
			}

			//Set remaining Lerp Variables
			_lerpTime = _isLerping ? fillDelay : 0;
			_lerpTargetValue = newValue;
			_lerpIsGain = isGain;
			_isLerping = true;
		}

		public void SetValueInstantly(float newValue)
        {
			if (newValue == ProgressBar.Value) return;

			_isLerping = false;

			ProgressBar.SetValue(newValue, TextDisplayMode, PrefixText);

			if (previewFillImage != null) previewFillImage.fillAmount = ProgressBar.FillAmount;
		}

        private void Update()
        {
            if (_isLerping && previewFillImage != null)
            {
				//Update Time
				_lerpTime += Time.deltaTime;
				float time = _lerpTime - fillDelay;

				//Get Lerp Value
				_currentLerpValue = Mathf.Lerp(_lerpStartValue, _lerpTargetValue, time * fillPerSecond);

				//Display Value with correct Mode
				if (_lerpIsGain)
				{
					ProgressBarTextMode mode = TextUpdateMode == ResourceTextUpdateMode.Instant ? ProgressBarTextMode.KeepLastText : TextDisplayMode;
					ProgressBar.SetValue(_currentLerpValue, mode, PrefixText);
				}
                else
                {
					previewFillImage.fillAmount = _currentLerpValue / ProgressBar.MaxValue;
					if (TextUpdateMode == ResourceTextUpdateMode.Lerp) ProgressBar.OverwriteText(_currentLerpValue, TextDisplayMode, PrefixText);
				}

				//End Lerp if Target was reached
				if (_currentLerpValue == _lerpTargetValue)
                {
					_isLerping = false;
					ProgressBar.SetValue(_lerpTargetValue, TextDisplayMode, PrefixText);
					previewFillImage.fillAmount = ProgressBar.FillAmount;
				}
			}
        }
	}

	public enum ResourceTextUpdateMode
    {
		Instant,
		Lerp,
    }
}