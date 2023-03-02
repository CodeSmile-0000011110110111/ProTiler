using SBG.Toolbelt.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBG.Toolbelt.Demo
{
	public class ResourceDisplayDemo : MonoBehaviour
	{
        [SerializeField] ResourceBarUI resourceBar;
        [SerializeField] ResourceCounterUI resourceCounter;
        [SerializeField] float value = 100;
        [SerializeField] float maxValue = 100;


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (resourceBar != null)
            {
                resourceBar.SetMaxValue(maxValue);
                resourceBar.SetValueInstantly(value);
            }

            if (resourceCounter != null)
            {
                resourceCounter.SetMaxValue(maxValue);
                resourceCounter.SetValue(value, true);
            }
        }

        public void ChangeValue(float increment)
        {
            value = Mathf.Clamp(value + increment, 0, maxValue);

            if (resourceBar != null)
            {
                resourceBar.SetValue(value);
            }

            if (resourceCounter != null)
            {
                resourceCounter.SetValue(value);
            }
        }

        public void SwitchToBar(ResourceBarUI newBar)
        {
            resourceBar?.gameObject.SetActive(false);
            resourceCounter?.gameObject.SetActive(false);

            resourceCounter = null;
            resourceBar = newBar;
            resourceBar.gameObject.SetActive(true);

            Init();
        }

        public void SwitchToCounter(ResourceCounterUI newCounter)
        {
            resourceBar?.gameObject.SetActive(false);
            resourceCounter?.gameObject.SetActive(false);

            resourceBar = null;
            resourceCounter = newCounter;
            resourceCounter.gameObject.SetActive(true);

            Init();
        }
    }
}