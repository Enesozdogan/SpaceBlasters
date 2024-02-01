using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerClient : IDisposable
{
    private const string MenuScene = "MenuScene";
    private const string GameScene = "GameScene";

    private JoinAllocation joinAlloc;
    private ClientNetworking clientNetwork;
    private MatchplayMatchmaker matchmaker;
    private UserData user_data;

    /// <summary>
    /// Unity servislerini baslatir ve Auth durumunu kontrol eden cagriyi baslatir.
    /// </summary>
    /// <returns></returns>
    public async Task<bool> InitializeAsync()
    {
       await UnityServices.InitializeAsync();
       clientNetwork = new ClientNetworking(NetworkManager.Singleton);
       matchmaker = new MatchplayMatchmaker();
       if(await AuthHelper.DoAuth() == AuthState.Success)
        {
            user_data = new UserData()
            {
                userName = PlayerPrefs.GetString(PlayerNameSetter.Name, "Default"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            return true;
        }
       return false;
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(MenuScene);
    }

    /// <summary>
    /// Lobi kodunu kullanarak belirtilen lobide client oyuncusunu baslatir.
    /// Kullanici bilgilerini User_Data altinda paketler ve server'a payload olarak yollar.
    /// </summary>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    public async Task InitiateClient(string joinCode)
    {
        try
        {
            joinAlloc = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            throw;
        }

        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(joinAlloc, "dtls");
        unityTransport.SetRelayServerData(relayServerData);

        StartClientConnection();

    }

    private void StartClientConnection()
    {
        string payloadObj = JsonUtility.ToJson(user_data);
        byte[] payloadObjByteArr = Encoding.UTF8.GetBytes(payloadObj);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadObjByteArr;

        NetworkManager.Singleton.StartClient();
    }
    public void InitiateClientOnServer(string ip,int port)
    {
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        unityTransport.SetConnectionData(ip,(ushort)port);
        StartClientConnection();
    }
    public async Task<MatchmakerPollingResult> FindMatch()
    {
        MatchmakingResult matchMakingResult = await matchmaker.Matchmake(user_data);

        if(matchMakingResult.result == MatchmakerPollingResult.Success)
        {
            InitiateClientOnServer(matchMakingResult.ip, matchMakingResult.port);
        }
        return matchMakingResult.result;
    }

    public async void FindMatchUI(Action<MatchmakerPollingResult> OnMatchResponse)
    {
        if (matchmaker.IsMatchmaking)
            return;

        MatchmakerPollingResult result = await FindMatch();
        OnMatchResponse?.Invoke(result);
    }
    public async Task StopMatchMaking()
    {
       await matchmaker.CancelMatchmaking();
    }
    public void Dispose()
    {
        clientNetwork?.Dispose();
    }

    public void DisConnectFromGame()
    {
        clientNetwork.DisConnectFromGame();
    }

  
}
