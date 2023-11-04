using System.Collections;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class Fireball : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        private IEnumerator Start()
        {
            _particleSystem.Stop();
            yield return new WaitForSeconds(0.65f);
            // Starts the RayCast process to look for enemy on layer 6
            Vector3 center = this.transform.position;
            int maxColliders = 10;
            Collider[] enemyHit = new Collider[maxColliders];
            int numColliders = Physics.OverlapSphereNonAlloc(center, 4, enemyHit, 1 << 6);
            for (int i = 0; i < numColliders; i++) {
                enemyHit[i].GetComponent<Enemies.EnemyAI>().Damage(100);
            }
            _particleSystem.Play();
            Destroy(gameObject, 2.5f);
        }
    }
}