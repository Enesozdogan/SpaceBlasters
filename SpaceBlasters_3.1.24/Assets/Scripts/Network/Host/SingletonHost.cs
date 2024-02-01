using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class SingletonHost : MonoBehaviour
{
    private static SingletonHost instance;
    public GameManagerHost GameManager { get; private set; }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static SingletonHost Instance { 
        
        get { 
           
            if(instance != null) return instance;

            instance= FindObjectOfType<SingletonHost>();
            if(instance == null)
            {
                Debug.LogError("No Host Singleton Object Found on the scene");   
            }
           return instance; 

        }


    }


    public void InstantiateHostManager()
    {
        GameManager= new GameManagerHost();
    }
    private void OnDestroy()
    {
        GameManager?.Dispose();

    }
}
