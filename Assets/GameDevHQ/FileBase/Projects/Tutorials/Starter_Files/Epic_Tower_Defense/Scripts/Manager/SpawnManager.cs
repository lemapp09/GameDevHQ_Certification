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
        [SerializeField] private int _howManyEnemyToSPawn, howManyEnemyToSpawn;
        [SerializeField] private int _currentEnemyToSpawn, _currentLevel;
        [SerializeField] private float _timeBetweenSpawns, timeBetweenSpawns;
        [SerializeField] private bool _isLevelOver;
        private float _timeToNextSpawn = -1f;   // Seeds the Spawn countdown to Spawn immediately

        /// <summary>
        /// Connects to events
        /// </summary>
        private void OnEnable() {
            GameManager.StartLevel += StartNewLevel;
            GameManager.StartPlay += StartSpawningEnemies;
            GameManager.PauseLevel += StopSpawningEnemies;
            GameManager.UnpauseLevel += StartSpawningEnemies;
            GameManager.StopLevel += RePoolEnemies;
            GameManager.RestartLevel += RePoolEnemies;
        }
        
        public void StartNewLevel()
        {
            _currentEnemyToSpawn = 0;
            _currentLevel = GameManager.Instance.GetCurrentLevel();
            _isLevelOver = false;
            StopSpawningEnemies();
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
            timeBetweenSpawns = _timeBetweenSpawns - _currentLevel * 0.01f;
            howManyEnemyToSpawn = _howManyEnemyToSPawn + _currentLevel * 20;
            GameManager.Instance.SetNumberOfEnemy(howManyEnemyToSpawn);
            while (!_isLevelOver) {
                if (_timeToNextSpawn < 0f && _currentEnemyToSpawn < howManyEnemyToSpawn) {
                    _timeToNextSpawn = timeBetweenSpawns;
                    GameObject temp = PoolManager.Instance.GetPooledObject();
                    temp.transform.position = GenerateSpawnPoint();
                    temp.transform.SetParent(this.transform);
                    temp.name = "Citizen" + Random.Range(0,99).ToString();
                    temp.GameObject().SetActive(true);
                    temp.GameObject().GetComponent<Enemies.EnemyAI>().InitializeVariables();
                    _currentEnemyToSpawn++;
                }
                if (gameObject.transform.childCount == 0) {
                        GameManager.Instance.AllEnemyKilled();
                        _isLevelOver = true;
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
            _isLevelOver = true;
            List<GameObject> temp = new List<GameObject>();
            foreach (Transform child in this.transform) {
                child.gameObject.transform.position = GenerateSpawnPoint();
                temp.Add(child.gameObject);
            }
            for (int i = 0; i < temp.Count; i++){
                PoolManager.Instance.ReturnToPool(temp[i]);
            }
            _currentEnemyToSpawn = 0;
        }

        /// <summary>
        /// Stop listening to events
        /// </summary>
        private void OnDisable()
        {
            GameManager.StartLevel -= StartNewLevel;
            GameManager.StartPlay -= StartSpawningEnemies;
            GameManager.PauseLevel -= StopSpawningEnemies;
            GameManager.UnpauseLevel -= StartSpawningEnemies;
            GameManager.StopLevel -= RePoolEnemies;
            GameManager.RestartLevel -= RePoolEnemies;
        }
    }
}