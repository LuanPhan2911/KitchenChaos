using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }
    public static bool isMultiplayer;
    public const int MAX_PLAYER_IN_GAME = 4;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChange;

    [SerializeField] private KitchenObjectListSO kitchenObjectListSO;

    private NetworkList<PlayerData> playerDataNetworkList;

    [SerializeField] List<Color> playerColorList;

    private string playerName;
    private const string PLAYER_PREFS_PLAYER_NAME = "PlayerNameMultiplier";
    private void Awake()

    {
        Instance = this;
        playerDataNetworkList = new NetworkList<PlayerData>();

        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME,
        $"PlayerName{UnityEngine.Random.Range(0, 1000)}");


        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
        DontDestroyOnLoad(gameObject);

    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChange?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        if (!isMultiplayer)
        {
            // single player
            StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
        }
    }

    public string GetPlayerName()
    {
        return playerName;
    }
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, this.playerName);

    }


    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }
    private void NetworkManager_ConnectionApprovalCallback(
          NetworkManager.ConnectionApprovalRequest approvalRequest,
           NetworkManager.ConnectionApprovalResponse approvalResponse)
    {
        if (!isMultiplayer)
        {
            approvalResponse.Approved = true;
            return;
        }

        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.CharacterSelectScene.ToString())
        {
            approvalResponse.Approved = false;
            approvalResponse.Reason = "Game has already started";

            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_IN_GAME)
        {
            approvalResponse.Approved = false;
            approvalResponse.Reason = "Game is full";

            return;
        }
        approvalResponse.Approved = true;
    }

    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            colorId = GetFirstUnusedColorId()
        });
        SetPlayerNameServerRPC(GetPlayerName());
        SetPlayerIdServerRPC(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRPC(GetPlayerName());
        SetPlayerIdServerRPC(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRPC(string playerName, ServerRpcParams serverRpcParams = default)
    {

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRPC(string playerId, ServerRpcParams serverRpcParams = default)
    {

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        int kitchenObjectSOIndex = GetKitchenObjectSOIndex(kitchenObjectSO);
        SpawnKitchenObjectServerRPC(kitchenObjectSOIndex, kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRPC(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetKitchenObjectSoFromIndex(kitchenObjectSOIndex);

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();
        if (kitchenObjectParent.HasKitchenObject())
        {
            //player has kitchen object
            return;
        }

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();


        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    }
    public KitchenObjectSO GetKitchenObjectSoFromIndex(int kitchenObjectSOIndex)
    {
        return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRPC(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRPC(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        if (kitchenObjectNetworkObject == null)
        {
            //this kitchen object already destroy
            return;
        }

        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRPC(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRPC(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerDataNetworkList.Count;
    }
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }
    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public Color GetPlayerColor(int colorId)
    {
        return playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorServerRPC(colorId);
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerColorServerRPC(int colorId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            // color is used
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.colorId = colorId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    public bool IsColorAvailable(int colorId)
    {
        foreach (var playerData in playerDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                // color is already in use
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }



}
