using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientNetworking  : IDisposable
{
    private NetworkManager networkManager;
    private const string MenuScene = "MenuScene";

    public ClientNetworking(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDC;
    }

    /// <summary>
    /// Server uzerinden kopan bir client bilgileri veri yapisindan cikarilir.
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDC(ulong clientId)
    {
        //ClientId = 0 ise host, 
        if (clientId != 0 && networkManager.LocalClientId != clientId) return;
        DisConnectFromGame();
     

    }

    public void Dispose()
    {
        if(networkManager!= null)
        {
            networkManager.OnClientDisconnectCallback -= OnClientDC;
        }
    }

    public void DisConnectFromGame()
    {
        if (SceneManager.GetActiveScene().name != MenuScene)
        {
            SceneManager.LoadScene(MenuScene);
        }
        if (networkManager.IsConnectedClient)
        {
            networkManager.Shutdown();
        }
    }
}
