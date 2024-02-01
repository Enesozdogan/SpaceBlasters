using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Networking.Transport.Error;
using Unity.Services.Core;
using UnityEngine;
using Unity.Netcode;
public class SingletonServer : MonoBehaviour
{
    private static SingletonServer instance;
    public GameManagerServer GameManager { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static SingletonServer Instance
    {

        get
        {

            if (instance != null) return instance;

            instance = FindObjectOfType<SingletonServer>();
            if (instance == null)
            {
                Debug.LogError("No Server Singleton Object Found on the scene");
            }
            return instance;

        }


    }


    public async Task InstantiateServerManager(NetworkObject soldierPrefab)
    {
        await UnityServices.InitializeAsync();
        GameManager = new GameManagerServer(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton,
            soldierPrefab
        );
    }
    private void OnDestroy()
    {
        GameManager?.Dispose();

    }
}
