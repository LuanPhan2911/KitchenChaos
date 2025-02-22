using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour
{

    public static KitchenGameLobby Instance { get; private set; }

    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler OnHideLobbyMessage;
    public event EventHandler<OnLobbyListChangeEventArgs> OnLobbyListChange;
    public class OnLobbyListChangeEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";

    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyListTimer;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializedUnityAuthentication();
    }
    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodLobbyList();
    }

    private void HandlePeriodLobbyList()
    {
        if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn &&
         SceneManager.GetActiveScene().name == SceneLoader.Scene.LobbyScene.ToString())
        {
            lobbyListTimer -= Time.deltaTime;
            if (lobbyListTimer <= 0f)
            {
                float lobbyListTimerMax = 3f;
                lobbyListTimer = lobbyListTimerMax;
                LobbyList();
            }
        }
    }


    private async void HandleHeartbeat()
    {
        if (!IsLobbyHost())
        {
            return;
        }
        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer <= 0f)
        {
            float heartbeatTimerMax = 15f;
            heartbeatTimer = heartbeatTimerMax;

            try
            {
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    private async void LobbyList()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),

                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(
                   queryLobbiesOptions
               );
            OnLobbyListChange?.Invoke(this, new OnLobbyListChangeEventArgs
            {
                lobbyList = queryResponse.Results
            });

        }
        catch (LobbyServiceException e)
        {

            Debug.Log(e);
        }
    }
    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitializedUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());
            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }
    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_IN_GAME - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;

        }
    }
    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;

        }
    }
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            return await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName,
                KitchenGameMultiplayer.MAX_PLAYER_IN_GAME,
              new CreateLobbyOptions
              {
                  IsPrivate = isPrivate,


              });
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id,
             new UpdateLobbyOptions
             {
                 Data = new Dictionary<string, DataObject>
                 {
                   {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                 }
             });
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                new RelayServerData(allocation, "dtls")
            );
            KitchenGameMultiplayer.Instance.StartHost();
            OnHideLobbyMessage?.Invoke(this, EventArgs.Empty);

            SceneLoader.LoadNetwork(SceneLoader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
               new RelayServerData(joinAllocation, "dtls")
           );

            OnHideLobbyMessage?.Invoke(this, EventArgs.Empty);
            KitchenGameMultiplayer.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {

            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);

        }
    }
    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
               new RelayServerData(joinAllocation, "dtls")
           );

            OnHideLobbyMessage?.Invoke(this, EventArgs.Empty);
            KitchenGameMultiplayer.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);

        }
    }
    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
               new RelayServerData(joinAllocation, "dtls")
           );
            OnHideLobbyMessage?.Invoke(this, EventArgs.Empty);
            KitchenGameMultiplayer.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
            Debug.Log(e);

        }
    }
    public async void DeleteLobby()
    {
        if (joinedLobby == null)
        {
            return;
        }
        try
        {

            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void LeaveLobby()
    {
        if (joinedLobby == null)
        {
            return;
        }
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void KickPlayer(string playerId)
    {
        if (!IsLobbyHost())
        {
            return;
        }
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    public Lobby GetLobby()
    {
        return joinedLobby;
    }
}
