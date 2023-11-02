using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MetroMayhem.Manager
{

    public class PoolManager : MonoSingleton<PoolManager>
    {
        #region Variables
        [Header("Enemy Pool Settings")]
        [SerializeField] private List<GameObject> _listOfEnemyPrefabs;
        [SerializeField] private List<Transform> _waypoints;
        [SerializeField] private GameObject pooledEnemyObject; // Prefab to be pooled
        [SerializeField] private int pooledEnemyAmount; // Initial pooled amount
        [SerializeField] private bool willEnemyGrow = true; // Should the pool grow?
        private List<GameObject> _pooledEnemyObjects;
        private Queue<int> _prefabEnemyQueue;
        private List<int> _tempList;
        private int[] _hashCodes;
        #endregion
        
        private void OnEnable() {
            _hashCodes = new int[8];
            GetAnimatorHashCodes();
        }

        void Start() {
            EnemyStart();
        }

        private void EnemyStart() {
            _pooledEnemyObjects = new List<GameObject>();
            _prefabEnemyQueue = new Queue<int>();
            _tempList = new List<int>();
            for (int i = 0; i < pooledEnemyAmount; i++) {
                GameObject obj = InstantiateEnemy();
            }
        }

        // Get an object from the pool
        public GameObject GetPooledObject()
        {
            // Search for an inactive object
            for (int i = 0; i < _pooledEnemyObjects.Count; i++) {
                if (!_pooledEnemyObjects[i].activeInHierarchy) {
                    return _pooledEnemyObjects[i];
                }
            }
            // If no inactive object is found and pool should grow, create a new object
            if (willEnemyGrow) {
                return InstantiateEnemy();
            }
            return null;
        }
        
        // Return an object to the pool
        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            _pooledEnemyObjects.Add(obj);
        }
        
        public int GetNextPrefabID()
        {
            if (_prefabEnemyQueue.Count == 0) ShuffleIDQueue();
            int value = _prefabEnemyQueue.Dequeue();
            return value;
        }

        private void ShuffleIDQueue()
        {
            if (_prefabEnemyQueue.Count != 0) return;
            _tempList.Clear();
            for(int i = 0; i < _listOfEnemyPrefabs.Count; i++)
            {
                _tempList.Add(i);
            }

            while(_tempList.Count > 0)
            {
                var randomIndex = Random.Range(0, _tempList.Count);
                _prefabEnemyQueue.Enqueue(_tempList[randomIndex]);
                _tempList.RemoveAt(randomIndex);
            }
        }

        private GameObject InstantiateEnemy()
        {
            GameObject obj = Instantiate(_listOfEnemyPrefabs[GetNextPrefabID()]);
            Enemies.EnemyAI tempEnemyAI = obj.GetComponent<Enemies.EnemyAI>();
            tempEnemyAI.PopulateWaypoints(_waypoints);
            tempEnemyAI.InitializeHashCodes(_hashCodes);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            _pooledEnemyObjects.Add(obj);
            return obj;
        }
        
        private void GetAnimatorHashCodes() {
            // Get Animator Hash Codes to better communicate with the Animator
            _hashCodes[0] = Animator.StringToHash("Idles");
            _hashCodes[1] = Animator.StringToHash("Hits");
            _hashCodes[2] = Animator.StringToHash("Deaths");
            _hashCodes[3] = Animator.StringToHash("IdleNumber");
            _hashCodes[4] = Animator.StringToHash("Death");
            _hashCodes[5] = Animator.StringToHash("Hit");
            _hashCodes[6] = Animator.StringToHash("Speed");
            _hashCodes[7] = Animator.StringToHash("Attack");
        }
    }
}
