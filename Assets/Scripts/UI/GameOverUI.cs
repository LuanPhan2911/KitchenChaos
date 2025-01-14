using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI deliveredText;
    [SerializeField] private Button playAgainButton;
    private void Start()
    {


        GameManager.Instance.OnStateChanged += (sender, args) =>
        {
            if (GameManager.Instance.IsGameOver())
            {
                Show();
                deliveredText.text = DeliveryManager.Instance.GetDeliveredSuccess().ToString();
            }
            else
            {
                Hide();
            }
        };
        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.MainMenuScene);
        });

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    private void Show()
    {

        gameObject.SetActive(true);
        playAgainButton.Select();
    }
}
