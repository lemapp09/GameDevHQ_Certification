using System;
using MetroMayhem.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class DetectEnemy : MonoBehaviour
    {
        [SerializeField] private IWeapon _weapon; // The weapon that this script is attached to.
        private bool _isActive; // Only detect enemy when the game is being played
        private float _timeBetweenRaycasts = 0;  // tracks time since last raycast
        [SerializeField] private float _raycastDelay = 1f; // The delay between each raycast
        [SerializeField] private float _radius;
        [SerializeField] private LayerMask _enemyLayer; // The layer that contains the enemies
        
        private void OnEnable()
        {    // The GameManager script sends out broadcast messages about the status of the game
            GameManager.StartLevel += Inactive;
            GameManager.StartPlay += Active;
            GameManager.PauseLevel += Inactive;
            GameManager.UnpauseLevel += Active;
            GameManager.StopLevel += Inactive;
            GameManager.RestartLevel += Inactive;
            if (_weapon == null)
            {    // Grabs the IWeapon component on this GameObject
                _weapon = GetComponent<IWeapon>();
            }
        }

        private void Active()
        {    // Starts the RayCast process to look for enemy
            _isActive = true;
        }

        private void Update()
        {
            _timeBetweenRaycasts += Time.deltaTime; // Increment the Time delay on each frame
            if(_isActive == true && _timeBetweenRaycasts >= _raycastDelay)
            {    // Starts the RayCast process to look for enemy on layer 6
                Vector3 center = this.transform.position;
                int maxColliders = 10;
                Collider[] enemyHit = new Collider[maxColliders];
                int numColliders = Physics.OverlapSphereNonAlloc(center, _radius, enemyHit, _enemyLayer);
                for (int i = 0; i < numColliders; i++)
                {
                    enemyHit[i].GetComponent<Enemies.EnemyAI>().Attack();
                    _weapon.Damage(1);
                }
                _timeBetweenRaycasts = 0;
            }
        }

        private void Inactive()
        {    // Stops the RayCast process to look for enemy
            _isActive = false;
        }

        private void OnDisable() {
            GameManager.StartLevel -= Inactive;
            GameManager.StartPlay -= Active;
            GameManager.PauseLevel -= Inactive;
            GameManager.UnpauseLevel -= Active;
            GameManager.StopLevel -= Inactive;
            GameManager.RestartLevel -= Inactive;
        }
    }
}