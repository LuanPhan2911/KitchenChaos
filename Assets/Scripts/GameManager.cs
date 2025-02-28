using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{

    [SerializeField] private Transform playerPrefab;

    public enum State
    {
        waitingToStart,
        countdownToStart,
        gamePlaying,
        gameOver
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.waitingToStart);



    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float gamePlayingTimerMax = 300f;

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private bool isLocalPaused;

    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;

    public event EventHandler OnGamePaused;

    public event EventHandler OnGameUnpaused;
    private bool isLocalPlayerReady;

    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playerPausedDictionary = new Dictionary<ulong, bool>();

    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private bool autoTestPaused = false;




    private void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += (previousValue, newValue) =>
        {
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        };
        isGamePaused.OnValueChanged += (previousValue, newValue) =>
        {
            if (isGamePaused.Value)
            {
                Time.timeScale = 0f;
                OnGamePaused?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Time.timeScale = 1f;
                OnGameUnpaused?.Invoke(this, EventArgs.Empty);
            }
        };
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += (clientId) =>
           {
               autoTestPaused = true;
           };
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;

        }
    }
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        }
    }
    private void Start()
    {
        GameInput.Instance.OnPauseAction += (sender, args) =>
        {
            TogglePauseGame();
        };
        GameInput.Instance.OnInteractAction += (sender, args) =>
        {

            if (state.Value == State.waitingToStart)
            {
                isLocalPlayerReady = true;
                OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
                SetPlayerReadyServerRPC();


            }
        };

    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRPC(ServerRpcParams serverRpcParams = default)
    {
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
            SetState(State.countdownToStart);
        }
    }
    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case State.waitingToStart:

                break;
            case State.countdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    gamePlayingTimer.Value = gamePlayingTimerMax;
                    SetState(State.gamePlaying);
                }
                break;
            case State.gamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    SetState(State.gameOver);
                }
                break;
            case State.gameOver:
                GameData.Instance.SetMaxRecipeCompleted(
                    DeliveryManager.Instance.GetDeliveredSuccess()
                    );
                break;

        }


    }
    private void LateUpdate()
    {
        if (autoTestPaused)
        {
            autoTestPaused = false;
            TestPlayerPausedGame();
        }
    }
    public void AddGamePlayingTimer(float second)
    {
        gamePlayingTimer.Value += second;
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.gamePlaying;
    }
    public bool IsCountdownGameStart()
    {
        return state.Value == State.countdownToStart;
    }
    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }
    public bool IsWaitingToStart()
    {
        return state.Value == State.waitingToStart;
    }

    private void SetState(State state)
    {
        this.state.Value = state;

    }
    public float GetCountdownTimer()
    {
        return countdownToStartTimer.Value;
    }
    public float GetGamePlayingTimer()
    {
        return gamePlayingTimer.Value;
    }
    public bool IsGameOver()
    {
        return state.Value == State.gameOver;
    }
    public float GetGamePlayingNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }
    public void TogglePauseGame()
    {
        isLocalPaused = !isLocalPaused;
        if (isLocalPaused)
        {

            OnGamePausedServerRPC();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);

        }
        else
        {

            OnGameUnpausedServerRPC();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);

        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnGamePausedServerRPC(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestPlayerPausedGame();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnGameUnpausedServerRPC(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestPlayerPausedGame();

    }
    private void TestPlayerPausedGame()
    {
        foreach (ulong clientId in NetworkManager.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                // has player pause game
                isGamePaused.Value = true;
                return;
            }
        }

        // all player not pause game
        isGamePaused.Value = false;
    }


}
