using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MetroMayhem.Manager
{

    public class PoolManager : MonoSingleton<PoolManager>
    {
        #region Variables
        [SerializeField] private List<GameObject> _listOfEnemyPrefabs;
        [SerializeField] private List<Transform> _waypoints;
        public GameObject pooledObject; // Prefab to be pooled
        public int pooledAmount; // Initial pooled amount
        public bool willGrow = true; // Should the pool grow?
        private List<GameObject> _pooledObjects;
        private Queue<int> _prefabQueue;
        private List<int> _tempList;
        private int[] _hashCodes;
        #endregion
        
        private void OnEnable() {
            _hashCodes = new int[8];
            GetAnimatorHashCodes();
        }

        void Start() {
            _pooledObjects = new List<GameObject>();
            _prefabQueue = new Queue<int>();
            _tempList = new List<int>();
            for (int i = 0; i < pooledAmount; i++) {
                GameObject obj = InstantiateEnemy();
            }
        }

        // Get an object from the pool
        public GameObject GetPooledObject()
        {
            // Search for an inactive object
            for (int i = 0; i < _pooledObjects.Count; i++) {
                if (!_pooledObjects[i].activeInHierarchy) {
                    return _pooledObjects[i];
                }
            }
            // If no inactive object is found and pool should grow, create a new object
            if (willGrow) {
                return InstantiateEnemy();
            }
            return null;
        }
        
        // Return an object to the pool
        public void ReturnToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            _pooledObjects.Add(obj);
        }
        
        public int GetNextPrefabID()
        {
            if (_prefabQueue.Count == 0) ShuffleIDQueue();
            int value = _prefabQueue.Dequeue();
            return value;
        }

        private void ShuffleIDQueue()
        {
            if (_prefabQueue.Count != 0) return;
            _tempList.Clear();
            for(int i = 0; i < _listOfEnemyPrefabs.Count; i++)
            {
                _tempList.Add(i);
            }

            while(_tempList.Count > 0)
            {
                var randomIndex = Random.Range(0, _tempList.Count);
                _prefabQueue.Enqueue(_tempList[randomIndex]);
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
            _pooledObjects.Add(obj);
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
