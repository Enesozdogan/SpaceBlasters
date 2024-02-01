using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay;
using UnityEngine;
using System;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine.UIElements;
using System.Text;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker.Models;

public class GameManagerServer : IDisposable
{
    private string server_ip;
    private int server_port;
    private int query_port;
    public ServerNetworking ServerNetworking { get; private set;}
    private MultiplayAllocationService multiplayAllocationService;
    private MatchplayBackfiller matcMakingBackFiller;
   
    public GameManagerServer(string server_ip, int server_p, int qport, NetworkManager networkManager)
    {
        this.server_ip = server_ip;
        server_port = server_p;
        query_port = qport;
        ServerNetworking = new ServerNetworking(networkManager);
        multiplayAllocationService = new MultiplayAllocationService();

    }



    public async Task InitiateGameServer()
    {
        await multiplayAllocationService.BeginServerCheck();

        try
        {
            MatchmakingResults results = await FetchMatchMakingPayload();
            if(results!=null)
            {
                await BeginBackFilling(results);
                ServerNetworking.OnActionClientJoinMatch += OnPlayerConnect;
                ServerNetworking.OnActionClientLeaveMatch += OnPlayerDisconnect;
            }
            else
            {
                Debug.LogWarning("No payload from matchmaker");
            }
        }
        catch (Exception e )
        {

            Debug.LogError(e);
        }

        bool val = ServerNetworking.CreateConnection(server_ip, server_port);
        if (!val)
        {
            Debug.LogError("Server couldnt start");
            return;
        }
        Debug.LogWarning("Bu sahne yukleniyor");
       
    }

    private async Task BeginBackFilling(MatchmakingResults results)
    {
        matcMakingBackFiller = new MatchplayBackfiller($"{server_ip}:{server_port}", results.QueueName,results.MatchProperties, 20);

        if (matcMakingBackFiller.NeedsPlayers())
        {
            await matcMakingBackFiller.BeginBackfilling();
        }

    }

    private void OnPlayerConnect(UserData userData)
    {
        Debug.LogWarning(userData);
        matcMakingBackFiller.AddPlayerToMatch(userData);
        multiplayAllocationService.AddPlayer();

        if (!matcMakingBackFiller.NeedsPlayers() && matcMakingBackFiller.IsBackfilling)
            _= matcMakingBackFiller.StopBackfill();
    }
    private void OnPlayerDisconnect(UserData userData)
    {
        int playerNum =matcMakingBackFiller.RemovePlayerFromMatch(userData.userAuthId);
        multiplayAllocationService.RemovePlayer();

        if (playerNum <= 0)
        {
            //Server Shutdown
            ShutdownServer();
        }
        if (matcMakingBackFiller.NeedsPlayers() && !matcMakingBackFiller.IsBackfilling)
        {
            _ = matcMakingBackFiller.BeginBackfilling();
        }
    }

    private async void ShutdownServer()
    {
        await matcMakingBackFiller.StopBackfill();
        Dispose();
        Application.Quit();
    }

    private async Task<MatchmakingResults> FetchMatchMakingPayload()
    {
        //On Matchmaking subscribe to events like OnMultiplayalloc
        Task<MatchmakingResults> matchPayload =  multiplayAllocationService.SubscribeAndAwaitMatchmakerAllocation();

        if(await Task.WhenAny(matchPayload, Task.Delay(20000)) == matchPayload)
        {
            return matchPayload.Result;
        }
        return null;
    }
    public void Dispose()
    {
        ServerNetworking.OnActionClientJoinMatch -= OnPlayerConnect;
        ServerNetworking.OnActionClientLeaveMatch -= OnPlayerDisconnect;
        matcMakingBackFiller?.Dispose();
        multiplayAllocationService?.Dispose();
        ServerNetworking?.Dispose();
        
    }



}
