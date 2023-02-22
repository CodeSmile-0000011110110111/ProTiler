using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {
    /// <summary>
    /// Moving and rotating platforms.
    /// </summary>
    public class Platform : MonoBehaviour
    {
        [System.Serializable]
        public enum Mode
        {
            Random = 0,
            Velocity = 1,
        }

        public Mode mode;
        [ShowIf("mode", Mode.Velocity)] public Vector3 velocity;
        [ShowIf("mode", Mode.Random)] public float randomMag = 10f;
        [ShowIf("mode", Mode.Random)] public float randomTime = 3f;
        [ShowIf("mode", Mode.Random)] public float moveSpeed = 5f;

        private Vector3 defaultPos;
        private Vector3 targetPosition;
        private Rigidbody r;

        public BehaviourPuppet[] puppetsOnPlatform;
        public Rigidbody[] rigidbodiesOnPlatform;


        void Start()
        {
            r = GetComponent<Rigidbody>();

            // Store defaults
            defaultPos = transform.position;
            targetPosition = transform.position;

            // Start switching target position
            StartCoroutine(NewTargetPos());
        }

        void FixedUpdate()
        {
            // Moving
            switch(mode)
            {
                case Mode.Random:
                    r.MovePosition(Vector3.SmoothDamp(r.position, targetPosition, ref velocity, 1f, moveSpeed));
                    break;
                case Mode.Velocity:
                    r.MovePosition(r.position + velocity * Time.deltaTime);
                    break;
            }
            

            foreach (Rigidbody rP in rigidbodiesOnPlatform)
            {
                rP.MovePosition(rP.position + velocity * Time.deltaTime);
            }

            foreach (BehaviourPuppet puppet in puppetsOnPlatform)
            {
                puppet.platformVelocity = velocity;
            }
        }

        // Switching the  target rotation
        private IEnumerator NewTargetPos()
        {
            while (true)
            {
                switch (mode)
                {
                    case Mode.Random:
                        yield return new WaitForSeconds(Random.value * randomTime);
                        Vector3 randomPos = Random.insideUnitSphere * randomMag;
                        randomPos.y = 0f;
                        targetPosition = defaultPos + randomPos;
                        break;
                    case Mode.Velocity:
                        yield return null;
                        break;
                }
            }
        }
    }
}

