using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LeaveButton : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            SingletonHost.Instance.GameManager.ShutdownHost();
        }
        SingletonClient.Instance.GameManager.DisConnectFromGame();
    }
}
