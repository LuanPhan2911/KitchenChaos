using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;

    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField playerNameInputField;

    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.LeaveLobby();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });
        lobbyTemplate.gameObject.SetActive(false);


    }
    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((newPlayerName) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(newPlayerName);
        });
        KitchenGameLobby.Instance.OnLobbyListChange += KitchenGameLobby_OnLobbyListChange;

        UpdateLobbyList(new List<Lobby>());
    }
    private void KitchenGameLobby_OnLobbyListChange(object sender, KitchenGameLobby.OnLobbyListChangeEventArgs args)
    {
        UpdateLobbyList(args.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate)
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        KitchenGameLobby.Instance.OnLobbyListChange -= KitchenGameLobby_OnLobbyListChange;
    }
}
