using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SBG.Toolbelt.UI
{
    [Serializable]
	public class ScreenFade
	{
        [SerializeField] private Image fadePanel;


        public void Init()
        {
            DisableInstantly();
        }

        public void SetColor(Color color)
        {
            fadePanel.color = color;
        }

        public void DisableInstantly()
        {
            if (!fadePanel.enabled) return;

            fadePanel.color = Color.clear;
            fadePanel.raycastTarget = false;
            fadePanel.enabled = false;
        }

        public IEnumerator CO_ExecuteFade(bool transitionToColor, Color color, float duration, float startDelay, Action callback, Action<Color> internalCallback)
        {
            Color startColor = fadePanel.color;
            float time = -startDelay;

            //Enable Color Screen for Lerp
            fadePanel.enabled = true;
            fadePanel.raycastTarget = true;
            fadePanel.color = startColor;
            yield return null; //Somehow waiting a frame helps the the time to work properly

            //EXECUTE FADE LERP
            if (duration > 0)
            {
                while (time < duration)
                {
                    fadePanel.color = Color.Lerp(startColor, color, time / duration);

                    yield return null;
                    time += Time.deltaTime;
                }
            }

            //Clamp to Target Value
            fadePanel.color = color;

            //Disable Screen if game is visible
            if (!transitionToColor)
            {
                fadePanel.enabled = false;
                fadePanel.raycastTarget = false;
            }

            //Invoke Callbacks
            internalCallback?.Invoke(color);
            callback?.Invoke();
        }
    }
}