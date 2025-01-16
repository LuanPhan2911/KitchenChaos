using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour
{

    [SerializeField] private Button playAgainButton;
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnect;

        playAgainButton.onClick.AddListener(() =>
       {

           SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
       });
        Hide();
    }
    private void NetworkManager_OnClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            Show();
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


}
