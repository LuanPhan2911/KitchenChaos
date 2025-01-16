using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        Hide();
    }
    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, EventArgs args)
    {
        ShowMessage("Creating lobby...");
    }
    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, EventArgs args)
    {
        ShowMessage("Create lobby failed");
    }
    private void KitchenGameLobby_OnJoinStarted(object sender, EventArgs args)
    {
        ShowMessage("Joining lobby...");
    }

    private void KitchenGameLobby_OnQuickJoinFailed(object sender, EventArgs args)
    {
        ShowMessage("Could not find a lobby in quick join");
    }
    private void KitchenGameLobby_OnJoinFailed(object sender, EventArgs args)
    {
        ShowMessage("Fail to join lobby");
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, EventArgs args)
    {
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
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
    }
}
