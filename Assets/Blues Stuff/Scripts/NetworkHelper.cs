using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Netcode;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using System.Collections;

public class NetworkHelper : MonoBehaviour
{
    public static NetworkHelper Singleton;

    public PlayerNetworkManager PlayerNetworkManager;
    public bool DevelopementBuild = true;

    [HideInInspector]
    [Tooltip("The current lobby the player is connected to")]
    public Lobby Lobby;

    [HideInInspector]
    [Tooltip("The current player's Auth ID")]
    public string PlayerID;
    
    string JoinCodeKey = "a";

    [HideInInspector]
    [Tooltip("The current lobby the player is connected to")]
    UnityTransport _transport;
    private async void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Debug.LogError("Blue you suck balls");
        }

        InitializationOptions options = new InitializationOptions();

        if (DevelopementBuild)
        {
            options.SetProfile(UnityEngine.Random.Range(float.MinValue, float.MaxValue).ToString());
        }

        await UnityServices.InitializeAsync(options);
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
         
        PlayerID = AuthenticationService.Instance.PlayerId;

        _transport = GetComponent<UnityTransport>();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public async Task JoinClient(Action<bool> callback, string joinCode)
    {
        JoinAllocation a;

        try
        {
            try
            {
                Lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);
            }
            catch (Exception e)
            {
                MenuManager.Singleton.ThrowErrorSFX(e);
                return;
            }
            a = await RelayService.Instance.JoinAllocationAsync(Lobby.Data[JoinCodeKey].Value);
        }
        catch (Exception e)
        {
            MenuManager.Singleton.ThrowErrorSFX(e);
            callback(false);
            return;
        }

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        callback(NetworkManager.Singleton.StartClient());
    }

    public async Task JoinHost(Action<bool, string> callback, bool isPrivate, Difficulty difficulty)
    {
        Allocation a;

        try
        {
            a = await RelayService.Instance.CreateAllocationAsync(2);
        }
        catch (Exception e)
        {
            MenuManager.Singleton.ThrowErrorSFX(e);
            callback(false, "");
            return;
        }

        string relayJoinCode;

        try
        {
            relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        }
        catch (Exception e)
        {
            MenuManager.Singleton.ThrowErrorSFX(e);
            callback(false, "");
            return;
        }

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject> {
                {
                    JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode)
                }
            }
        };

        lobbyOptions.IsPrivate = isPrivate;

        try
        {
            Lobby = await Lobbies.Instance.CreateLobbyAsync("New Lobby", 2, lobbyOptions);
        }
        catch (Exception e)
        {
            MenuManager.Singleton.ThrowErrorSFX(e);
            callback(false, "");
            return;
        }

        StartCoroutine(LobbyHeartbeat(Lobby.Id, 15));

        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        callback(NetworkManager.Singleton.StartHost(), Lobby.LobbyCode);
    }

    IEnumerator LobbyHeartbeat(string lobbyID, float waitTime)
    {
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
    private void OnClientConnect(ulong clientID)
    {
        Debug.Log("Someone Connected");
        if (NetworkManager.Singleton.IsServer)
        {
            if (clientID == NetworkManager.Singleton.LocalClientId)
            {
                // As Host, host connected to server
            }
            else
            {
                // As Host, Client connected to server

                PlayerNetworkManager pnm = Instantiate(PlayerNetworkManager);
                pnm.NetworkObject.SpawnWithOwnership(clientID, false);

                pnm = Instantiate(PlayerNetworkManager);
                pnm.NetworkObject.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId, false);
            }
        }
        else
        {
            // As Client, Client connected to server
        }
    }

    private void OnClientDisconnect(ulong clientID)
    {

    }
}