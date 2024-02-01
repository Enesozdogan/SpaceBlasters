using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyElement : MonoBehaviour
{
    [SerializeField] TMP_Text lobbyNameTxt;
    [SerializeField] TMP_Text maxPlayerTxt;

    private Lobby lobby;
    private LobbyList lobbyList;


    public void InitLobbyItem(LobbyList lobbyList, Lobby lobby)
    {
        this.lobbyList = lobbyList;
        this.lobby = lobby;

        lobbyNameTxt.text = lobby.Name;
        maxPlayerTxt.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

    }

    //Her ayri katil butonuna baglanan ve lobi listesi sinifi uzerinde lobby verisi gondererek katilim yapan fonk.
    public void JoinLobby()
    {
        if (lobbyList != null)
        {
            lobbyList.JoinLobby(lobby);
        }
    }
}
