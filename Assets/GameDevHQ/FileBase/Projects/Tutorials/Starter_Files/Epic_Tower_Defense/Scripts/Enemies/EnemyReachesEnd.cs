using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroMayhem.Enemies
{


    public class EnemyReachesEnd : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                other.GetComponent<EnemyAI>().ReachedEnd();
            }
        }
    }
}