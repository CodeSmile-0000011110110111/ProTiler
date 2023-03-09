using UnityEngine;

namespace SBG.Toolbelt
{
	[RequireComponent(typeof(Collider))]
    public class Trigger3D : Trigger
	{
        private void OnTriggerEnter(Collider other)
        {
            ProcessEvent(TriggerEvent.OnTriggerEnter, other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            ProcessEvent(TriggerEvent.OnTriggerExit, other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            ProcessEvent(TriggerEvent.OnTriggerStay, other.gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            ProcessEvent(TriggerEvent.OnCollisionEnter, collision.collider.gameObject);
        }

        private void OnCollisionExit(Collision collision)
        {
            ProcessEvent(TriggerEvent.OnCollisionExit, collision.collider.gameObject);
        }

        private void OnCollisionStay(Collision collision)
        {
            ProcessEvent(TriggerEvent.OnCollisionStay, collision.collider.gameObject);
        }
    }
}