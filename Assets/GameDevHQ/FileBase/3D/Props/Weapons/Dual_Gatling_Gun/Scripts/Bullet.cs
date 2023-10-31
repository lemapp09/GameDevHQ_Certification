using UnityEngine;

namespace GameDevHQ.FileBase.Dual_Gatling_Gun
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 20f;
        public Rigidbody rb;

        void Start() {
            rb.velocity = transform.forward * speed;
            Invoke("DestroyBullet", 0.5f);
        }
        
        private void DestroyBullet() {
            Destroy(gameObject);
        }
    }
}