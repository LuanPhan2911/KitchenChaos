using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{

    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;

    private void Awake()
    {

        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            KitchenGameLobby.Instance.KickPlayer(playerData.playerId.ToString());
            KitchenGameMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {

        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChange += KitchenGameMultiplayer_OnPlayerDataNetworkListChange;

        CharacterSelectReady.Instance.OnReadyChanged += (sender, args) =>
        {
            UpdatePlayer();
        };




        UpdatePlayer();
    }
    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChange(object sender, EventArgs eventArgs)
    {
        UpdatePlayer();
    }
    private void UpdatePlayer()
    {
        if (KitchenGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = KitchenGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(
                playerData.clientId
            ));
            playerNameText.text = playerData.playerName.ToString();

            playerVisual.SetPlayerColor(KitchenGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));

            ulong clientId = playerData.clientId;
            ulong serverClientId = NetworkManager.ServerClientId;


            kickButton.gameObject.SetActive(clientId != serverClientId && NetworkManager.Singleton.IsServer);
        }
        else
        {
            Hide();
        }
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
        KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChange -= KitchenGameMultiplayer_OnPlayerDataNetworkListChange;
    }
}
