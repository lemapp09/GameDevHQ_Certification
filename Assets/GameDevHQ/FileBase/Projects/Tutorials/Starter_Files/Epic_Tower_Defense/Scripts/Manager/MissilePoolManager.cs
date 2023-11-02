using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissilePoolManager : MonoSingleton<MissilePoolManager>
{

    public GameObject objectPrefab;
    public int poolSize = 20;
    [SerializeField] private bool willMissileGrow = true; // Should the pool grow?
    private Queue<GameObject> objectPool = new Queue<GameObject>();

    void OnEnable() {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            InstantiateMissile();
        }
    }

    public GameObject GetMissile()
    {
        if (objectPool.Count > 0)
        {
            GameObject obj = objectPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        // If no inactive object is found and pool should grow, create a new object
        if (willMissileGrow) {
            GameObject obj = Instantiate(objectPrefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
            return InstantiateMissile();
        }
        return null;
    }

    private GameObject InstantiateMissile()
    {
        GameObject obj = Instantiate(objectPrefab);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
        return obj;
    }

    public IEnumerator ReturnMissile(GameObject obj, float destroyerTimer)
    {
        yield return  new WaitForSeconds(destroyerTimer);
        obj.SetActive(false);
        objectPool.Enqueue(obj);
        yield return new WaitForSeconds(Time.deltaTime);
    }
}