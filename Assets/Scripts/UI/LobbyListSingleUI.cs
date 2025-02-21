using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{

    private Lobby Lobby;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyPlayerCountText;
    [SerializeField] private Button joinButton;



    private void Awake()
    {
        joinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithId(Lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.Lobby = lobby;
        lobbyNameText.text = lobby.Name;

        lobbyPlayerCountText.text = $"Player {lobby.Players.Count}/{lobby.MaxPlayers}";
    }
}
