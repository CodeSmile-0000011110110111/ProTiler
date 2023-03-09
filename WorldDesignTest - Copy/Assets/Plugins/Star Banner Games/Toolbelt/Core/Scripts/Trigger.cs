using UnityEngine;
using UnityEngine.Events;

namespace SBG.Toolbelt
{
	public abstract class Trigger : MonoBehaviour
	{
        protected enum TriggerEvent
        {
            OnTriggerEnter,
            OnTriggerExit,
            OnTriggerStay,
            OnCollisionEnter,
            OnCollisionExit,
            OnCollisionStay,
        }

        [SerializeField] private bool filterByTag = false;
        [SerializeField] private string[] tagFilters;
        [SerializeField] private bool filterByLayer = false;
        [SerializeField] private LayerMask layerMask;

        public UnityEvent OnTriggerEnterEvent;
        public UnityEvent OnTriggerStayEvent;
        public UnityEvent OnTriggerExitEvent;

        public UnityEvent OnCollisionEnterEvent;
        public UnityEvent OnCollisionStayEvent;
        public UnityEvent OnCollisionExitEvent;


        protected void ProcessEvent(TriggerEvent triggerEvent, GameObject hitObject)
        {
            if (!IsValidHit(hitObject)) return;

            switch (triggerEvent)
            {
                case TriggerEvent.OnTriggerEnter:   OnTriggerEnterEvent?.Invoke(); break;
                case TriggerEvent.OnTriggerExit:    OnTriggerExitEvent?.Invoke(); break;
                case TriggerEvent.OnTriggerStay:    OnTriggerStayEvent?.Invoke(); break;
                case TriggerEvent.OnCollisionEnter: OnCollisionEnterEvent?.Invoke(); break;
                case TriggerEvent.OnCollisionExit:  OnCollisionExitEvent?.Invoke(); break;
                case TriggerEvent.OnCollisionStay:  OnCollisionStayEvent?.Invoke(); break;
                default: break;
            }
        }

        private bool IsValidHit(GameObject hitObject)
        {
            if (filterByTag && !IsValidTag(hitObject.tag)) return false;
            if (filterByLayer && !THelper.IsLayerInLayermask(hitObject.layer, layerMask)) return false;

            return true;
        }

        private bool IsValidTag(string target)
        {
            bool filteredOut = true;

            foreach (string tag in tagFilters)
            {
                if (tag == target) filteredOut = false;
            }

            return filteredOut;
        }
    }
}