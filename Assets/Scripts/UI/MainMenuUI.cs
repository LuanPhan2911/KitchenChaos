using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singlePlayerButton;
    [SerializeField] private Button quitButton;


    private void Awake()
    {
        multiplayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.isMultiplayer = true;
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
        });
        singlePlayerButton.onClick.AddListener(() =>
        {
            KitchenGameMultiplayer.isMultiplayer = false;
            SceneLoader.Load(SceneLoader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        Time.timeScale = 1f;
        multiplayerButton.Select();
    }
}
