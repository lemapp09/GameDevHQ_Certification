using UnityEngine;

namespace GameDevHQ.FileBase.Dual_Gatling_Gun
{
    [RequireComponent(typeof(Rigidbody))]
    public class MissileProjectile : MonoBehaviour
    {
        public float speed = 10f;
        public float rotationSpeed = 200f;
        public Transform target;

        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

            rb.velocity = transform.forward * speed;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.transform == target)
            {
                // Handle collision with the target, e.g., apply damage or create an explosion effect
                Destroy(gameObject);
            }
        }
    }
}