using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// ObjectPool design pattern, abstract class.

public abstract class ObjectPool<T>  : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
    [SerializeField] protected T prefab;
    private List<T> pooledObjects;
    private int amount; 
    private bool isReady;

    // Create with n amount of objects.
    public void PoolObjects(int amount = 0)  // like, Player has box thats nothing in it. 
    {
        if(amount<0)
            throw new ArgumentOutOfRangeException("Amount of pool must be non-negative");
        
        this.amount= amount;    

        pooledObjects = new List<T>(amount);

        GameObject newObject;
        for(int i=0; i < amount; i++)
        {
            newObject = Instantiate(prefab.gameObject, transform);
            newObject.SetActive(false);
            pooledObjects.Add(newObject.GetComponent<T>());
        }

        // give ready flag 
        isReady = true;
    }


    public virtual T GetPooledObject() 
    {
        if (!isReady)
            PoolObjects(1);
        for (int i =0; i != amount; ++i)
            if(!pooledObjects[i].isActiveAndEnabled) 
                return pooledObjects[i];

        GameObject newObject = Instantiate(prefab.gameObject, transform);
        newObject.SetActive(false);
        pooledObjects.Add(newObject.GetComponent<T>());
        ++amount; // because amount is 0 by default.
        return newObject.GetComponent<T>();
    }


    public virtual void ReturnObjectToPool(T returningObject)
    {
        // verify 
        if(returningObject == null)
            return;

        if (!isReady)
        {
            PoolObjects();
            pooledObjects.Add(returningObject);
            amount++;
        }
        returningObject.gameObject.SetActive(false);
    }
}
