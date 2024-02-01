using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] LobbyElement lobbyElementGO;
    [SerializeField] Transform lobbySpawnPoint;
    private bool canJoin = true;
    private bool canRefresh = true;



    
    /// <summary>
    /// Lobi Elementi uzerinden gelen lobi bilgisi ile yeni client'i ekler.
    /// </summary>
    /// <param name="lobby"></param>
    public async void JoinLobby(Lobby lobby)
    {
        if (!canJoin) return;
        canJoin = false;


        try
        {
         
            Lobby lobbyTmp = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);

            string codeToJoin = lobbyTmp.Data["JoinCode"].Value;
            await SingletonClient.Instance.GameManager.InitiateClient(codeToJoin);
        }
        catch (Exception e)
        {

            Debug.LogError(e);
        }
    }

    private void OnEnable()
    {
        RefreshLobbyList();
    }

    /// <summary>
    /// Filtreleme yaparak lobi listesini gunceller.
    /// </summary>
    public async void RefreshLobbyList()
    {
        if (!canRefresh) return;
        canRefresh = false;



        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 20;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field:QueryFilter.FieldOptions.AvailableSlots,
                    op:QueryFilter.OpOptions.GT,
                    value:"0")
            };

            QueryResponse lobbyELements = await Lobbies.Instance.QueryLobbiesAsync(options);
            RemakeList(lobbyELements);

        }
        catch (Exception e)
        {

            Debug.LogError(e);
        }



        canRefresh = true;
    }

    /// <summary>
    /// Lobi elementlerini bastan uretir.
    /// </summary>
    /// <param name="lobbies"></param>
    private void RemakeList(QueryResponse lobbies)
    {
        foreach (Transform child in lobbySpawnPoint)
        {
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbies.Results)
        {
            LobbyElement lobbyInstance=Instantiate(lobbyElementGO, lobbySpawnPoint);
            lobbyInstance.InitLobbyItem(this, lobby);

        }
    }
}
