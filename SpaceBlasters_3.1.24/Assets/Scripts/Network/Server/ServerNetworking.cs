using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ServerNetworking : IDisposable
{
    private NetworkManager networkManager;
    Dictionary<ulong, string> clientId_Auth = new Dictionary<ulong, string>();
    Dictionary<string, UserData> clientAuth_Data = new Dictionary<string, UserData>();

    public Action<string> OnActionClientLeave;
    public Action<UserData> OnActionClientLeaveMatch;
    public Action<UserData> OnActionClientJoinMatch;
    public ServerNetworking(NetworkManager networkManager)
    {
        this.networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ConnApprovedCheck;
        networkManager.OnServerStarted += OnNetworkStart;
    }


    /// <summary>
    /// Baglanti dogrulamasi gerceklenir ve userdata alinir.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    private void ConnApprovedCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payloadObj = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payloadObj);



        clientId_Auth[request.ClientNetworkId] = userData.userAuthId;
        clientAuth_Data[userData.userAuthId] = userData;

        OnActionClientJoinMatch?.Invoke(userData);

        response.Approved = true;
        response.Position = RespawnLocation.GenerateRandomPos();
        response.Rotation = Quaternion.identity;
        response.CreatePlayerObject = true;


    }

    public UserData GetUserData(ulong clientNetworkId)
    {
        if (clientId_Auth.TryGetValue(clientNetworkId, out string auth))
        {
            //return clientAuth_Data[auth];
            if (clientAuth_Data.TryGetValue(auth, out UserData userData))
            {
                return userData;
            }
        }
        return null;
    }
    private void OnNetworkStart()
    {
        networkManager.OnClientDisconnectCallback += OnClientDC;
    }

    /// <summary>
    /// Server uzerinden kopan bir client bilgileri veri yapisindan cikarilir.
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDC(ulong clientId)
    {
        if (clientId_Auth.TryGetValue(clientId, out string authId))
        {
            clientId_Auth.Remove(clientId);
            OnActionClientLeaveMatch?.Invoke(clientAuth_Data[authId]);
            clientAuth_Data.Remove(authId);
            OnActionClientLeave?.Invoke(authId);
        }
    }
    /// <summary>
    /// Server baslatimi icin Baglanti kuran fonksiyondur.
    /// </summary>
    public bool CreateConnection(string ip, int portNum)
    {
        UnityTransport unityTransport = networkManager.gameObject.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip, (ushort)portNum);
        return networkManager.StartServer();
    }
    public void Dispose()
    {
        if (networkManager == null) return;

        networkManager.OnClientDisconnectCallback -= OnClientDC;
        networkManager.ConnectionApprovalCallback -= ConnApprovedCheck;
        networkManager.OnServerStarted -= OnNetworkStart;


        if (networkManager.IsListening) networkManager.Shutdown();
    }
}
