using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;

    [SerializeField] private Button optionsButton;

    public static GamePauseUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        GameManager.Instance.OnLocalGamePaused += (sender, args) => Show();
        GameManager.Instance.OnLocalGameUnpaused += (sender, args) => Hide();

        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });

        resumeButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });

        optionsButton.onClick.AddListener(() =>
        {
            Hide();
            OptionsUI.Instance.Show();
        });

        Hide();



    }
    public void Show()
    {
        gameObject.SetActive(true);
        resumeButton.Select();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
