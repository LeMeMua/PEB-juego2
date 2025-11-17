using System.Collections.Generic;
using UnityEngine;

public class waterdropplet : MonoBehaviour
{
    public GameObject dropletPrefab;
    public int poolSize = 200;

    private List<GameObject> pool;

    void Awake()
    {
        pool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(dropletPrefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public GameObject Get()
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
                return obj;
        }

        return null;
    }
}
