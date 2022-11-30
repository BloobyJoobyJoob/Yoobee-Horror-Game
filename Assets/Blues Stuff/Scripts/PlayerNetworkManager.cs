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

    NetworkVariable<FixedString32Bytes> Username = new NetworkVariable<FixedString32Bytes>();

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
        string un = PlayerPrefs.GetString("PlayerUsername", "RandomNoob" + UnityEngine.Random.Range(100, 999).ToString());

        if (IsOwner)
        {
            Username.Value = new FixedString32Bytes(un);
            MenuManager.Singleton.Player1.text = un;
        }
        else
        {
            MenuManager.Singleton.Player2.text = Username.Value.ToString();
            Username.OnValueChanged += OnUsernameChange;
        }
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
        MenuManager.Singleton.Player2.text = Username.Value.ToString();
    }
}
