using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool : MonoBehaviour
{
    [Serializable]
    public struct Pool
    {
        public Queue<GameObject> pooledObjects;
        public GameObject dropPrefab;
        public int poolSize;
    }

    [SerializeField] private Pool[] pools = null;

    private void Awake() 
    {
        for(int i = 0; i < pools.Length; i++)
        {
            pools[i].pooledObjects = new Queue<GameObject>();

            for(int j= 0; j < pools[i].poolSize; j++)
            {
                GameObject drop = Instantiate(pools[i].dropPrefab);
                drop.SetActive(false);
                pools[i].pooledObjects.Enqueue(drop);
            }
        }
    }

    public GameObject PoolDrop(Vector2 dropPosition, int dropType)
    {
        if(dropType >= pools.Length)
        {
            return null;
        }
        else
        {
            GameObject drop = pools[dropType].pooledObjects.Dequeue();
            drop.transform.position = dropPosition;
            pools[dropType].pooledObjects.Enqueue(drop);
            drop.SetActive(true);
            return drop;
        }
    }

    public GameObject RespawnDrop(Vector2 dropPosition, int dropType)
    {
        GameObject drop = pools[dropType].pooledObjects.Dequeue();
        drop.transform.position = dropPosition;
        drop.SetActive(true);
        return drop;
    }

    public void ReturnToPool(GameObject drop)
    {

        drop.GetComponent<DropController>().isMatched = false;
        drop.transform.position = Vector2.zero;
        drop.SetActive(false);
    }

}
