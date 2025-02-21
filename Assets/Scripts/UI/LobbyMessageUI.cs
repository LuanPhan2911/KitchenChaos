using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;


    private float timerMax = 2f;
    private float timer;



    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnHideLobbyMessage += KitchenGameLobby_OnHideLobbyMessage;
        Hide();
    }

    private void KitchenGameLobby_OnHideLobbyMessage(object sender, EventArgs e)
    {
        Hide();
    }

    private void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            Hide();
        }
    }

    private void KitchenGameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs args)
    {
        timer = 5f;
        ShowMessage("Creating lobby...");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, EventArgs args)
    {
        timer = 5f;
        ShowMessage("Joining lobby...");
    }
    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs args)
    {
        timer = timerMax;
        ShowMessage("Create lobby failed");
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs args)
    {
        timer = timerMax;
        ShowMessage("Could not find a lobby in quick join");
    }
    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs args)
    {
        timer = timerMax;
        ShowMessage("Fail to join lobby");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs args)
    {
        timer = timerMax;
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Fail to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }
    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;

    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayer_OnTryingToJoinGame;
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;

        KitchenGameLobby.Instance.OnHideLobbyMessage -= KitchenGameLobby_OnHideLobbyMessage;
    }
}
