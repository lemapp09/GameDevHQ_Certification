using UnityEngine;

namespace MetroMayhem.Enemies
{


    public class EnemyReachesEnd : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                var enemyAI = other.GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.ReachedEnd();
                }

            }
        }
    }
}