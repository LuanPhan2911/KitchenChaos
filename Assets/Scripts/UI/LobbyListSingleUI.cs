using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{

    private Lobby Lobby;
    [SerializeField] private TextMeshProUGUI lobbyNameText;


    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithId(Lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.Lobby = lobby;
        lobbyNameText.text = lobby.Name;
    }
}
