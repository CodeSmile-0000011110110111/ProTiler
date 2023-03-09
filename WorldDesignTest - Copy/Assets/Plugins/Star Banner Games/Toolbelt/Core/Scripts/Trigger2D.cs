using UnityEngine;

namespace SBG.Toolbelt
{
    [RequireComponent(typeof(Collider2D))]
    public class Trigger2D : Trigger
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            ProcessEvent(TriggerEvent.OnTriggerEnter, other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            ProcessEvent(TriggerEvent.OnTriggerExit, other.gameObject);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            ProcessEvent(TriggerEvent.OnTriggerStay, other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            ProcessEvent(TriggerEvent.OnCollisionEnter, collision.collider.gameObject);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            ProcessEvent(TriggerEvent.OnCollisionExit, collision.collider.gameObject);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            ProcessEvent(TriggerEvent.OnCollisionStay, collision.collider.gameObject);
        }
    }
}