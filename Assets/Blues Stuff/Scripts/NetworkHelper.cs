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
            catch
            {
                MenuManager.Singleton.ThrowErrorSFX(ConnectionFailType.LobbyConnectError);
                return;
            }
            a = await RelayService.Instance.JoinAllocationAsync(Lobby.Data[JoinCodeKey].Value);
        }
        catch
        {
            MenuManager.Singleton.ThrowErrorSFX(ConnectionFailType.RelayConnectError);
            return;
        }

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        callback(NetworkManager.Singleton.StartClient());
    }

    public async Task JoinHost(Action<bool, string> callback, bool isPrivate)
    {
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2);

        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject> {
                {
                    JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, relayJoinCode)
                }
            }
        };

        lobbyOptions.IsPrivate = isPrivate;

        Lobby = await Lobbies.Instance.CreateLobbyAsync("New Lobby", 2, lobbyOptions);

        StartCoroutine(LobbyHeartbeat(Lobby.Id, 15));

        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        callback(NetworkManager.Singleton.StartHost(), Lobby.LobbyCode);
    }
    public async Task JoinPublic(Action<bool, string> callback)
    {
        try
        {
            Lobby quickJoin = await Lobbies.Instance.QuickJoinLobbyAsync();
        }
        catch
        {
            await JoinHost(callback, false);
            return;
        }
        callback(true, "");
    }

    IEnumerator LobbyHeartbeat(string lobbyID, float waitTime)
    {
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        if (Lobby != null)
        {
            if (Lobby.HostId == PlayerID)
            {
                Lobbies.Instance.DeleteLobbyAsync(Lobby.Id);
            }
            else
            {
                Lobbies.Instance.RemovePlayerAsync(Lobby.Id, PlayerID);
            }
        }
    }
}

public enum ConnectionFailType {
    LobbyConnectError, RelayConnectError
}
