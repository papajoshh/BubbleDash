using UnityEngine;
using System.Collections.Generic;

public class SimpleObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject prefabToPool;
    public int poolSize = 20;
    public bool expandable = true;
    
    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> allObjects = new List<GameObject>();
    
    void Start()
    {
        // Pre-instantiate objects
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }
    
    GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefabToPool);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
        allObjects.Add(obj);
        return obj;
    }
    
    public GameObject GetObject()
    {
        if (pool.Count == 0)
        {
            if (expandable)
            {
                return CreateNewObject();
            }
            else
            {
                Debug.LogWarning("Pool exhausted!");
                return null;
            }
        }
        
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }
    
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;
        
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
    
    public void ReturnAll()
    {
        foreach (GameObject obj in allObjects)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                ReturnObject(obj);
            }
        }
    }
}