using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using System.Threading.Tasks;
using System;

public class NetworkHelper : MonoBehaviour
{
    public static NetworkHelper Singleton;

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

        _transport = GetComponent<UnityTransport>();

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task JoinAsClient(string joinCode, Action<bool> callback)
    {
        JoinAllocation a;

        try
        {
            a = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            MenuManager.Singleton.ThrowErrorSFX(ConnectionFailType.BadCode);
            return;
        }

        try
        {
            _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
        }
        catch
        {
            MenuManager.Singleton.ThrowErrorSFX(ConnectionFailType.BadConnect);
            return;
        }

        callback(NetworkManager.Singleton.StartClient());
    }

    public async Task JoinAsHost(Action<bool, string> callback)
    {
        Allocation a = await RelayService.Instance.CreateAllocationAsync(2);

        string joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        callback(NetworkManager.Singleton.StartHost(), joinCode);
    }
}

public enum ConnectionFailType {
    BadCode, BadConnect
}
