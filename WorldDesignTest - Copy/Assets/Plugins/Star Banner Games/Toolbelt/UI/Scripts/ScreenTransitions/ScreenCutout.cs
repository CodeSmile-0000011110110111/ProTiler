using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SBG.Toolbelt.UI
{
    [Serializable]
	public class ScreenCutout
	{
        [SerializeField] private Image cutoutPanel;


        public void Init()
        {
            DisableInstantly();
        }

        public void DisableInstantly()
        {
            if (!cutoutPanel.enabled) return;

            cutoutPanel.color = Color.clear;
            cutoutPanel.raycastTarget = false;
            cutoutPanel.enabled = false;
        }

        public void SetCutoutShape(Sprite cutoutShape)
        {
            if (cutoutPanel.sprite != cutoutShape)
                cutoutPanel.sprite = cutoutShape;
        }

        public void SetColor(Color color)
        {
            cutoutPanel.color = color;
        }

        public IEnumerator CO_ExecuteCutout(bool transitionToColor, float duration, float startDelay, Action callback, Action<Color> internalCallback)
        {
            float time = -startDelay;

            float startValue = transitionToColor ? 1.5f : 0;
            float targetValue = transitionToColor ? 0f : 1.5f;

            //Enable Color Screen for Lerp
            cutoutPanel.enabled = true;
            cutoutPanel.raycastTarget = true;
            cutoutPanel.material.SetFloat("_CutoutSize", startValue);
            yield return null; //Somehow waiting a frame helps the the time to work properly

            //EXECUTE CUTOUT LERP
            if (duration > 0)
            {
                while (time < duration)
                {
                    float cutoutValue = Mathf.Lerp(startValue, targetValue, time / duration);

                    cutoutPanel.material.SetFloat("_CutoutSize", cutoutValue);

                    yield return null;
                    time += Time.deltaTime;
                }
            }

            //Clamp to Target Value
            cutoutPanel.material.SetFloat("_CutoutSize", targetValue);

            //Disable Screen if game is visible
            if (!transitionToColor)
            {
                cutoutPanel.enabled = false;
                cutoutPanel.raycastTarget = false;
            }

            //Invoke Callbacks
            internalCallback?.Invoke(transitionToColor ? cutoutPanel.color : Color.clear);
            callback?.Invoke();
        }
    }
}