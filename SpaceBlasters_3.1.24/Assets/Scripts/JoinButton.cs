using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinButton : MonoBehaviour
{
   
    public void ClickToJoin()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void ClickToBeHost()
    {
        NetworkManager.Singleton.StartHost();
    }
}
