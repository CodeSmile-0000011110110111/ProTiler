using SBG.Toolbelt.UI;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SBG.Toolbelt.Demo
{
	public class ScreenTransitionDemo : MonoBehaviour
	{
		[SerializeField] float fadeDuration = 0.75f;
		[SerializeField] float startDelay = 0.25f;
		[SerializeField] float idleTime = 0.6f;


		public void RandomTransition()
        {
			int transitionType = Random.Range(0, 3);

            switch (transitionType)
            {
				case 0:
					ScreenTransition.FadeToColor(Random.ColorHSV(), fadeDuration, startDelay, GetRandomReturnTranstion());
					break;
				case 1:
					ScreenTransition.CutoutToColor(Random.ColorHSV(), fadeDuration, startDelay, GetRandomReturnTranstion());
					break;
				case 2:
					var preset = (ScreenWipe.EdgePresets)Random.Range(0, Enum.GetValues(typeof(ScreenWipe.EdgePresets)).Length);
					var dir = (ScreenWipe.WipeDirection)Random.Range(0, Enum.GetValues(typeof(ScreenWipe.WipeDirection)).Length);
					ScreenTransition.SetEdgeSprites(preset);
					ScreenTransition.WipeToColor(dir, Random.ColorHSV(), fadeDuration, startDelay, GetRandomReturnTranstion());
					break;
				default:
                    break;
            }
        }

		private Action GetRandomReturnTranstion()
        {
			int transitionType = Random.Range(0, 3);

			switch (transitionType)
			{
				case 0:
					return () => ScreenTransition.FadeToGame(fadeDuration, idleTime);
				case 1:
					return () => ScreenTransition.CutoutToGame(fadeDuration, idleTime);
				case 2:
					var preset = (ScreenWipe.EdgePresets)Random.Range(0, Enum.GetValues(typeof(ScreenWipe.EdgePresets)).Length);
					var dir = (ScreenWipe.WipeDirection)Random.Range(0, Enum.GetValues(typeof(ScreenWipe.WipeDirection)).Length);
					ScreenTransition.SetEdgeSprites(preset);
					return () => ScreenTransition.WipeToGame(dir, fadeDuration, idleTime);
				default:
					return null;
			}
		}

		public void FadeToColorAndBack()
        {
			ScreenTransition.FadeToColor(Random.ColorHSV(), fadeDuration, startDelay, () =>
			{
				ScreenTransition.FadeToGame(fadeDuration, idleTime);
			});
		}

		public void CutoutToColorAndBack()
		{
			ScreenTransition.CutoutToColor(Random.ColorHSV(), fadeDuration, startDelay, () =>
			{
				ScreenTransition.CutoutToGame(fadeDuration, idleTime);
			});
		}

		public void WipeToColorAndBack()
		{
			var preset = (ScreenWipe.EdgePresets)Random.Range(0, Enum.GetValues(typeof(ScreenWipe.EdgePresets)).Length);
			var dir = (ScreenWipe.WipeDirection)Random.Range(0, Enum.GetValues(typeof(ScreenWipe.WipeDirection)).Length);

			ScreenTransition.SetEdgeSprites(preset);

			ScreenTransition.WipeToColor(dir, Random.ColorHSV(), fadeDuration, startDelay, () =>
			{
				ScreenTransition.WipeToGame(dir, fadeDuration, idleTime);
			});
		}
	}
}