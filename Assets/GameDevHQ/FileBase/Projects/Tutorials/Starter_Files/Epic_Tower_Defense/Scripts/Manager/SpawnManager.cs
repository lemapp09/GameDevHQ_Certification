using System;
using System.Collections;
using System.Collections.Generic;
using MetroMayhem.Enemies;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MetroMayhem.Manager
{
    public class SpawnManager : MonoSingleton<SpawnManager>
    {
        private List<GameObject> _listOfEnemyPrefabs;
        [SerializeField] private int _howManyEnemyToSPawn;
        private int _currentEnemyToSpawn;
        [SerializeField] private float _timeBetweenSpawns;
        [SerializeField] private bool _isLevelOver;
        private float _timeToNextSpawn = -1f;   // Seeds the Spawn countdown to Spawn immediately

        /// <summary>
        /// Connects to events
        /// </summary>
        private void OnEnable() {
            GameManager.StartLevel += StartSpawningEnemies;
            GameManager.StartPlay -= StartSpawningEnemies;
            GameManager.PauseLevel += StopSpawningEnemies;
            GameManager.UnpauseLevel += StartSpawningEnemies;
            GameManager.StopLevel += RePoolEnemies;
            GameManager.RestartLevel += RePoolEnemies;
        }
        
        /// <summary>
        /// At both the Start of a Level and when a Level is
        /// restarted from a Pause, Spawn Manager will begin to
        /// spawn EnemyAI GameObjects
        /// </summary>
        public void StartSpawningEnemies() {
            StartCoroutine(Spawn());
        }

        /// <summary>
        /// During a Level, the Player can request a Pause,
        /// the Spawn Manager will stop spawning EnemyAI
        /// </summary>
        private void StopSpawningEnemies() {
            StopAllCoroutines();
        }
        
        /// <summary>
        /// This method requests an EnemyAi game object from the
        /// PoolManager and assigns it to the spawn point.
        /// </summary>
        private IEnumerator Spawn()
        {
            while (!_isLevelOver) {
                if (_timeToNextSpawn < 0f && _currentEnemyToSpawn < _howManyEnemyToSPawn) {
                    _timeToNextSpawn = _timeBetweenSpawns;
                    GameObject temp = PoolManager.Instance.GetPooledObject();
                    temp.transform.position = GenerateSpawnPoint();
                    temp.transform.SetParent(this.transform);
                    temp.name = "Citizen" + Random.Range(0,99).ToString();
                    temp.GameObject().SetActive(true);
                    temp.GameObject().GetComponent<Enemies.EnemyAI>().InitializeVariables();
                    _currentEnemyToSpawn++;
                }
                _timeToNextSpawn -= Time.deltaTime;
                yield return new WaitForSeconds(0.1f);
            }
            yield return null;
        }

        private Vector3 GenerateSpawnPoint()
        {
            return new Vector3(Random.Range(-1.468f, 0.005f), 0.64f, Random.Range(-0.76f, 0.737f));
        }
        
        /// <summary>
        /// At the end of the Level,
        /// EnemyAI GameObjects are re-assigned to the PoolManager
        /// </summary>
        private void RePoolEnemies()
        {
            List<GameObject> temp = new List<GameObject>();
            foreach (Transform child in transform)
            {
                temp.Add(child.gameObject);
                child.gameObject.SetActive(false);
                child.gameObject.transform.SetParent(PoolManager.Instance.transform);
                child.gameObject.transform.position = GenerateSpawnPoint();
            }
        }

        /// <summary>
        /// Stop listening to events
        /// </summary>
        private void OnDisable()
        {
            GameManager.StartLevel -= StopSpawningEnemies;
            GameManager.StartPlay -= StartSpawningEnemies;
            GameManager.PauseLevel -= StopSpawningEnemies;
            GameManager.UnpauseLevel -= StartSpawningEnemies;
            GameManager.StopLevel -= StopSpawningEnemies;
            GameManager.RestartLevel += RePoolEnemies;
        }
    }
}