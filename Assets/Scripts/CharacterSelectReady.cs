using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();

    public static CharacterSelectReady Instance { get; private set; }

    public event EventHandler OnReadyChanged;

    private void Awake()
    {
        Instance = this;
    }


    public void SetPlayerReady()
    {
        SetPlayerReadyServerRPC();
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRPC(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRPC(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool isAllPlayerReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                isAllPlayerReady = false;
                break;
            }
        }
        if (isAllPlayerReady)
        {
            SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
        }
    }
    [ClientRpc]
    private void SetPlayerReadyClientRPC(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }
}
