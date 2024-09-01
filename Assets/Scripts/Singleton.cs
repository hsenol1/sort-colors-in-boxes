using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // Since some of the objects are critical in game cycle, we do not want to allow inconsistency over them, like GameManager, GameGrid and MatchablePool...
    private static T instance; 

    public static T Instance 
    {
        get {
            if (instance == null) 
                Debug.LogError("No instance of" + typeof(T) + " exists in the scene.");   
            return instance;
        }
    }


    protected void Awake()
    {
        if (instance==null)
        {
            instance = this as T;
            Init();
        }
        else 
        {
            Debug.LogWarning("An instance of" + typeof(T) + " already exists in scene. Self-destructing");
            Destroy(this.gameObject);
        }

    }

    protected void OnDestroy() 
    {
        if(this==instance) 
            instance = null;
    }

    protected virtual void Init() 
    {
        // Override this virtual function in inherited classes
    }

     


}
