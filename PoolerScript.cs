using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolerScript : MonoBehaviour
{
    [HideInInspector]
    protected List<GameObject> pooledObjects;
    protected GameObject objectToPool;
    protected int amountToPool;

    protected virtual void InitialisePooler()
    {
        pooledObjects = new List<GameObject>();
        for(int i=0;i<amountToPool;i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public virtual GameObject GetPooledObject()
    {
        for(int i=0;i<pooledObjects.Count;i++)
        {
            if(!pooledObjects[i].activeInHierarchy){
                return pooledObjects[i];
            }
        }
        return null;
    }

    public virtual void ResetPool()
    {
        for(int i=0;i<pooledObjects.Count;i++)
        {
            if(pooledObjects[i].activeInHierarchy){
                pooledObjects[i].SetActive(false);
            }
        }
    }
}
