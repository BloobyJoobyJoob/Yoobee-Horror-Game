using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using System;

public class PlayerNetworkManager : NetworkBehaviour
{
    public static PlayerNetworkManager ClientManager = null;
    public static PlayerNetworkManager TeammateManager = null;

    NetworkVariable<FixedString32Bytes> Username = new NetworkVariable<FixedString32Bytes>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        if (IsOwner)
        {
            if (ClientManager == null)
            {
                ClientManager = this;
            }
            else
            {
                Debug.LogError("Blue you smelly fart");
                return;
            }
        }
        else
        {
            if (TeammateManager == null)
            {
                TeammateManager = this;
            }
            else
            {
                Debug.LogError("Blue you smelly fart");
                return;
            }
        }
    }
    public override void OnNetworkSpawn()
    {
        string un = PlayerPrefs.GetString("PlayerUsername", "Noob" + UnityEngine.Random.Range(100, 999).ToString());

        if (IsOwner)
        {
            Username.Value = new FixedString32Bytes(un);
            MenuManager.Singleton.ClientUsername.text = un;
        }
        else
        {
            MenuManager.Singleton.OpponentUsername.text = Username.Value.ToString();
            Username.OnValueChanged += OnUsernameChange;
        }
    }

    public void SetUsername(string name)
    {
        Username.Value = new FixedString32Bytes(name);
        MenuManager.Singleton.ClientUsername.text = name;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {

        }
        else
        {
            Username.OnValueChanged -= OnUsernameChange;
        }
    }

    private void OnUsernameChange(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        MenuManager.Singleton.OpponentUsername.text = Username.Value.ToString();
    }
}
