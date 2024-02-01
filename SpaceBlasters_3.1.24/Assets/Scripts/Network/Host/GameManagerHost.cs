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

public class GameManagerHost  : IDisposable
{
	private const int maxConnectionCount = 20;
    private const string GameScene = "GameScene";

	private string lobbyID;
    private string joinCodeString;
	private Allocation alloc;

	public ServerNetworking ServerNetwork { get; private set; }

    public void Dispose()
    {
		ShutdownHost();
    }

    public async Task InitiateHost()
	{
		try
		{
			alloc=await Relay.Instance.CreateAllocationAsync(maxConnectionCount);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
			throw;
		}

        try
        {
            joinCodeString = await Relay.Instance.GetJoinCodeAsync(alloc.AllocationId);
			Debug.Log(joinCodeString);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            throw;
        }

		UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

		//dtls daha guvenli calismazsa udp koy
		RelayServerData relayServerData = new RelayServerData(alloc, "dtls");

		unityTransport.SetRelayServerData(relayServerData);

		try
		{
			CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions();
			createLobbyOptions.IsPrivate = false;
			createLobbyOptions.Data = new Dictionary<string,DataObject>()
			{
				{
					"JoinCode", new DataObject(

                        visibility:DataObject.VisibilityOptions.Member,
						value:joinCodeString

                    )
					
						
					
				}
			};

			string userName = PlayerPrefs.GetString(PlayerNameSetter.Name, "Default");
            Lobby lobby= await Lobbies.Instance.CreateLobbyAsync(userName, maxConnectionCount, createLobbyOptions);
			lobbyID = lobby.Id;
			
			SingletonHost.Instance.StartCoroutine(SendLobbyPing(30));
		}
		catch (LobbyServiceException ex)
		{
			Debug.LogError(ex);
			return;
		}

		ServerNetwork = new ServerNetworking(NetworkManager.Singleton);

		

        UserData user_Data = new UserData()
        {
            userName = PlayerPrefs.GetString(PlayerNameSetter.Name, "Default"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };
        string payloadObj = JsonUtility.ToJson(user_Data);
        byte[] payloadObjByteArr = Encoding.UTF8.GetBytes(payloadObj);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadObjByteArr;

        NetworkManager.Singleton.StartHost();

        ServerNetwork.OnActionClientLeave += HandleOnClientLeave;

        NetworkManager.Singleton.SceneManager.LoadScene(GameScene, LoadSceneMode.Single);
		

    }

    private async void HandleOnClientLeave(string autharizationId)
    {
		try
		{
			await LobbyService.Instance.RemovePlayerAsync(lobbyID, autharizationId);
		}
		catch (LobbyServiceException e)
		{

			Debug.LogError(e);
		}
    }

    public async void ShutdownHost()
    {
        if (string.IsNullOrEmpty(lobbyID)) return;


        SingletonHost.Instance.StopCoroutine(nameof(SendLobbyPing));

        try
        {
            await Lobbies.Instance.DeleteLobbyAsync(lobbyID);
        }
        catch (LobbyServiceException e)
        {

            Debug.LogException(e);
        }
        lobbyID = string.Empty;

        ServerNetwork.OnActionClientLeave -= HandleOnClientLeave;
        ServerNetwork?.Dispose();
    }

    private IEnumerator SendLobbyPing(float timer)
	{
		WaitForSeconds latency = new WaitForSeconds(timer);
		while(true)
		{
			Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);
			yield return latency;
		}
	}
   
}
