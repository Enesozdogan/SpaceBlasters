using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SingletonClient : MonoBehaviour
{
    private static SingletonClient instance;
    public GameManagerClient GameManager { get; private set;}
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    //Singleton Gerceklemesi
    public static SingletonClient Instance { 
        get 
        {
            if (instance != null) return instance;
            
            instance= FindObjectOfType<SingletonClient>();
            if (instance == null)
            {
                Debug.LogError("No SingletonClient on scene");
                return null;
            }
            return instance; 
        
        } 
    
    }


    public async Task<bool> InstantiateClientManager()
    {
        GameManager= new GameManagerClient();
        return await GameManager.InitializeAsync();
    }
    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}


